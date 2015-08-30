using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;
using Knightware.Diagnostics;
using Spyder.Client.IO;
using Spyder.Client.Net.DrawingData.Deserializers;
using Knightware.Net.Sockets;

namespace Spyder.Client.Net.Notifications
{
    public delegate void ServerUpdateMessageHandler(object sender, SpyderServerAnnounceInformation serverInfo);
    public delegate void DrawingDataReceivedHandler(object sender, DrawingDataReceivedEventArgs e);
    public delegate void TraceLogMessageHandler(object sender, TraceLogMessageEventArgs e);

    /// <summary>
    /// Listens for UDP multicast / broadcast messages from Spyder servers
    /// </summary>
    public class SpyderServerEventListenerBase
    {
        public const string multicastIP = "239.192.25.25";
        public const int multicastPort = 11118;

        private IMulticastListener listener;
        private Func<IGZipStreamDecompressor> getDrawingDataDecompressor;
        private Dictionary<string, DrawingDataDeserializer> drawingDataDeserializers;
        private Dictionary<string, SpyderServerAnnounceInformation> cachedServerInfo;

        public bool IsRunning { get; private set; }

        public SpyderServerEventListenerBase(IMulticastListener listener, Func<IGZipStreamDecompressor> getDrawingDataDecompressor)
        {
            this.listener = listener;
            this.getDrawingDataDecompressor = getDrawingDataDecompressor;
        }

        public async Task<bool> Startup()
        {
            Shutdown();
            IsRunning = true;

            drawingDataDeserializers = new Dictionary<string, DrawingDataDeserializer>();
            cachedServerInfo = new Dictionary<string, SpyderServerAnnounceInformation>();

            listener.DataReceived += listener_DataReceived;
            await listener.Startup(multicastIP, multicastPort);

            return true;
        }

        public void Shutdown()
        {
            listener.DataReceived -= listener_DataReceived;
            listener.Shutdown();

            if (drawingDataDeserializers != null)
            {
                foreach (var deserializer in drawingDataDeserializers.Values)
                {
                    deserializer.DrawingDataDeserialized -= deserializer_DrawingDataDeserialized;
                }
                drawingDataDeserializers = null;
            }
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
                catch(Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to parse hostname: {1}", ex.GetType().Name, ex.Message);
                }

                //Write to local cache
                if (cachedServerInfo.ContainsKey(server.Address))
                    cachedServerInfo[server.Address] = server;
                else
                    cachedServerInfo.Add(server.Address, server);

                OnServerAnnounceMessageReceived(server);
            }
            else if (eventType.Value == ServerEventType.DrawingDataChanged)
            {
                //Ensure we have the server's version info in cache before we start trying to deserialize it's data
                var serverInfo = (cachedServerInfo.ContainsKey(e.SenderAddress) ? cachedServerInfo[e.SenderAddress] : null);
                if (serverInfo != null)
                {
                    DrawingDataDeserializer deserializer;
                    if (!drawingDataDeserializers.ContainsKey(e.SenderAddress))
                    {
                        deserializer = new DrawingDataDeserializer(e.SenderAddress, serverInfo.Version.ToShortString(), getDrawingDataDecompressor());
                        deserializer.DrawingDataDeserialized += deserializer_DrawingDataDeserialized;
                        drawingDataDeserializers.Add(e.SenderAddress, deserializer);
                    }
                    else
                    {
                        deserializer = drawingDataDeserializers[e.SenderAddress];
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

        void deserializer_DrawingDataDeserialized(object sender, DrawingData.DrawingData drawingData)
        {
            var deserializer = (DrawingDataDeserializer)sender;
            OnDrawingDataReceived(new DrawingDataReceivedEventArgs(deserializer.ServerIP, drawingData));
        }

        private ServerEventType? ParseHeader(byte[] data)
        {
            if (data == null || data.Length < 12)
                return null;

            var expected = new byte[] { (byte)'s', (byte)'p', (byte)'y', (byte)'d', (byte)'e', (byte)'r', 0x00 };
            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != data[i])
                    return null;
            }

            ServerEventType eventType = (ServerEventType)(data[10] | (data[11] << 8));
            return eventType;
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

        public event TraceLogMessageHandler TraceLogMessageReceived;
        protected void OnTraceLogMessageReceived(TraceLogMessageEventArgs e)
        {
            if (TraceLogMessageReceived != null)
                TraceLogMessageReceived(this, e);
        }
    }
}
