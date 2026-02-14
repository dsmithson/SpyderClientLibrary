using Knightware.Diagnostics;
using Knightware.Net;
using Knightware.Net.Sockets;
using Knightware.Threading.Tasks;
using Spyder.Client.Common;
using Spyder.Client.Net.DrawingData;
using Spyder.Client.Net.DrawingData.Deserializers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.Notifications
{
    public delegate void ServerUpdateMessageHandler(object sender, SpyderServerAnnounceInformation serverInfo);
    public delegate void DrawingDataReceivedHandler(object sender, DrawingDataReceivedEventArgs e);
    public delegate void TraceLogMessageHandler(object sender, TraceLogMessageEventArgs e);
    public delegate void DataObjectChangedHandler(object sender, DataObjectChangedEventArgs e);

    /// <summary>
    /// Listens for UDP multicast / broadcast messages from Spyder servers
    /// </summary>
    public class SpyderServerEventListener
    {
        private static SpyderServerEventListener sharedInstance;
        private static readonly AsyncLock sharedInstanceLock = new AsyncLock();

        public static async Task<SpyderServerEventListener> GetInstanceAsync()
        {
            if (sharedInstance == null)
            {
                using (await sharedInstanceLock.LockAsync())
                {
                    if (sharedInstance == null)
                    {
                        sharedInstance = new SpyderServerEventListener();
                        await sharedInstance.StartupAsync();
                    }
                }
            }
            return sharedInstance;
        }

        private class SpyderServerListenerState
        {
            public DrawingDataDeserializer Deserializer { get; set; }
            public SpyderServerAnnounceInformation ServerAnnounceInfo { get; set; }
            public DateTime? LastDrawingDataEventRaisedTime { get; set; }
        }

        public const string multicastIP = "239.192.25.25";
        public const int multicastPort = 11118;
        private readonly IMulticastListener listener;
        private Dictionary<string, SpyderServerListenerState> serverInfoCache;

        /// <summary>
        /// Defines a throttle for maximum drawing data event raising (per Spyder server).  Setting to 1 second, for example, will ensure DrawingData does not fire more than once per second.  Set to TimeSpan.Zero (default) to disable throttling.
        /// </summary>
        public TimeSpan DrawingDataThrottleInterval { get; set; } = TimeSpan.Zero;

        public bool IsRunning { get; private set; }

        public SpyderServerEventListener()
        {
            this.listener = new UDPMulticastListener();
        }

        public async Task<bool> StartupAsync()
        {
            await ShutdownAsync();
            IsRunning = true;

            serverInfoCache = new Dictionary<string, SpyderServerListenerState>();

            listener.DataReceived += listener_DataReceived;
            if (!await listener.StartupAsync(multicastIP, multicastPort))
            {
                TraceQueue.Trace(this, TracingLevel.Error, "Failed to startup listener.  Shutting down...");
                await ShutdownAsync();
                return false;
            }

            return true;
        }

        public async Task ShutdownAsync()
        {
            IsRunning = false;

            listener.DataReceived -= listener_DataReceived;
            await listener.ShutdownAsync();

            if (serverInfoCache != null)
            {
                foreach (var serverInfo in serverInfoCache.Values)
                {
                    if (serverInfo.Deserializer != null)
                        serverInfo.Deserializer.DrawingDataDeserialized -= deserializer_DrawingDataDeserialized;
                }
            }
            serverInfoCache = null;
        }

        private void listener_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!IsRunning)
                return;

            byte[] data = e.Data;
            ServerEventType? eventType = ParseHeader(data);
            if (eventType == null)
                return;

            if (eventType.Value == ServerEventType.DSCPing)
            {
                var server = new SpyderServerAnnounceInformation()
                {
                    Address = e.SenderAddress,
                    FrameID = data[7],
                    IsVersion3OrHigher = (data.Length >= 16 ? (data[15] > 0) : false),
                    HardwareType = (data.Length >= 17 ? (HardwareType)data[16] : HardwareType.Spyder300),
                    Version = new VersionInfo()
                    {
                        Major = data[8],
                        Minor = data[9],
                        Build = data[14]
                    }
                };

                //Servers at or above version 4.0.4 include their hostname
                try
                {
                    const int hostNameStartIndex = 17;
                    if (data.Length > hostNameStartIndex && data[hostNameStartIndex] != 0x00)
                    {
                        int endIndex = Array.IndexOf<byte>(data, 0x00, hostNameStartIndex);
                        if (endIndex >= 0)
                        {
                            int length = endIndex - hostNameStartIndex;
                            server.ServerName = UTF8Encoding.UTF8.GetString(data, hostNameStartIndex, length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to parse hostname: {1}", ex.GetType().Name, ex.Message);
                }

                //Write to local cache
                if (serverInfoCache.ContainsKey(server.Address))
                    serverInfoCache[server.Address].ServerAnnounceInfo = server;
                else
                    serverInfoCache.Add(server.Address, new SpyderServerListenerState() { ServerAnnounceInfo = server });

                OnServerAnnounceMessageReceived(server);
            }
            else if (eventType.Value == ServerEventType.DataObjectChanged)
            {
                DataType changeTypes = 0;
                changeTypes |= (DataType)(data[12]);
                changeTypes |= (DataType)(data[13] << 8);
                changeTypes |= (DataType)(data[14] << 16);
                changeTypes |= (DataType)(data[15] << 24);

                int dataObjectVersion = 0;
                dataObjectVersion |= (data[16]);
                dataObjectVersion |= (data[17] << 8);
                dataObjectVersion |= (data[18] << 16);
                dataObjectVersion |= (data[19] << 24);

                var dataObjectChangedArgs = new DataObjectChangedEventArgs(e.SenderAddress, dataObjectVersion, changeTypes);
                OnDataObjectChanged(dataObjectChangedArgs);
            }
            else if (eventType.Value == ServerEventType.DrawingDataChanged)
            {
                //Ensure we have the server's version info in cache before we start trying to deserialize it's data
                var serverInfo = (serverInfoCache.ContainsKey(e.SenderAddress) ? serverInfoCache[e.SenderAddress] : null);
                if (serverInfo != null)
                {
                    bool skipProcesing = false;
                    if (this.DrawingDataThrottleInterval != TimeSpan.Zero && serverInfo.LastDrawingDataEventRaisedTime != null)
                    {
                        //There is a throttle specified for our max presentation interval - check to see if enough time has passed since the last event raised
                        DateTime lastRaiseDate = serverInfo.LastDrawingDataEventRaisedTime.Value;
                        DateTime nextRaiseDate = lastRaiseDate.Add(this.DrawingDataThrottleInterval);
                        if (nextRaiseDate > DateTime.UtcNow)
                            skipProcesing = true;
                    }

                    if (!skipProcesing)
                    {
                        DrawingDataDeserializer deserializer;
                        if (serverInfo.Deserializer == null)
                        {
                            deserializer = new DrawingDataDeserializer(e.SenderAddress, serverInfo.ServerAnnounceInfo.Version.ToShortString());
                            deserializer.DrawingDataDeserialized += deserializer_DrawingDataDeserialized;
                            serverInfo.Deserializer = deserializer;
                        }
                        else
                        {
                            deserializer = serverInfo.Deserializer;
                        }

                        //Feed the data to the deserializer
                        try
                        {
                            deserializer.Read(data, 12);
                        }
                        catch (Exception ex)
                        {
                            TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while deserializing drawing data: {1}",
                                ex.GetType().Name, ex.Message);
                        }
                    }
                }
            }
            else if (DiagnosticMessageTypeInfo.IsDiagnosticMessage(eventType.Value))
            {
                TraceMessage msg = new TraceMessage();
                msg.Level = DiagnosticMessageTypeInfo.GetTracingLevel(eventType.Value);

                //Parse out Message
                int index = 12;
                StringBuilder builder = new StringBuilder();
                while (e.Data[index] != 0x00 && index < e.Data.Length)
                {
                    builder.Append((char)e.Data[index++]);
                }
                msg.Message = builder.ToString();
                index++;

                //Parse out sender
                builder.Clear();
                while (e.Data[index] != 0x00 && index < e.Data.Length)
                {
                    builder.Append((char)e.Data[index++]);
                }
                msg.Sender = builder.ToString();
                index++;

                //Raise notification event
                OnTraceLogMessageReceived(new TraceLogMessageEventArgs(e.SenderAddress, msg));
            }
        }

        void deserializer_DrawingDataDeserialized(object sender, DrawingDataDeserializedEventArgs e)
        {
            var deserializer = (DrawingDataDeserializer)sender;

            //Record the last date of DrawingDataRaised
            if (serverInfoCache.ContainsKey(deserializer.ServerIP))
            {
                var serverInfo = serverInfoCache[deserializer.ServerIP];
                if (serverInfo != null)
                    serverInfo.LastDrawingDataEventRaisedTime = DateTime.UtcNow;
            }

            //Raise the DrawingDataReceived event
            OnDrawingDataReceived(new DrawingDataReceivedEventArgs(deserializer.ServerIP, e.DrawingData, e.RawMessage));
        }

        private ServerEventType? ParseHeader(byte[] data)
        {
            if (data == null || data.Length < 12)
                return null;

            //Spyder studio (v5 and above) changes the spyder header to mantis, so we need to check both
            if ((data[0] == (byte)'s' && data[1] == (byte)'p' && data[2] == (byte)'y' && data[3] == (byte)'d' && data[4] == (byte)'e' && data[5] == (byte)'r') ||
               (data[0] == (byte)'m' && data[1] == (byte)'a' && data[2] == (byte)'n' && data[3] == (byte)'t' && data[4] == (byte)'i' && data[5] == (byte)'s') ||
               (data[0] == (byte)'e' && data[1] == (byte)'c' && data[2] == (byte)'h' && data[3] == (byte)'o' && data[4] == (byte)'p' && data[5] == (byte)'s'))
            {
                ServerEventType eventType = (ServerEventType)(data[10] | (data[11] << 8));
                return eventType;
            }
            return null;
        }

        /// <summary>
        /// Raised when a server sends an announcement message onto the network
        /// </summary>
        public event ServerUpdateMessageHandler ServerAnnounceMessageReceived;
        protected void OnServerAnnounceMessageReceived(SpyderServerAnnounceInformation serverInfo)
        {
            if (ServerAnnounceMessageReceived != null)
                ServerAnnounceMessageReceived(this, serverInfo);
        }

        public event DrawingDataReceivedHandler DrawingDataReceived;
        protected void OnDrawingDataReceived(DrawingDataReceivedEventArgs e)
        {
            if (DrawingDataReceived != null)
                DrawingDataReceived(this, e);
        }

        public event DataObjectChangedHandler DataObjectChanged;
        protected void OnDataObjectChanged(DataObjectChangedEventArgs e)
        {
            if (DataObjectChanged != null)
                DataObjectChanged(this, e);
        }

        public event TraceLogMessageHandler TraceLogMessageReceived;
        protected void OnTraceLogMessageReceived(TraceLogMessageEventArgs e)
        {
            if (TraceLogMessageReceived != null)
                TraceLogMessageReceived(this, e);
        }
    }
}
