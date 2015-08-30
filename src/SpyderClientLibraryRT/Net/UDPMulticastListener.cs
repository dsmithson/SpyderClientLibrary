using Knightware.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Spyder.Client.Net
{
    public class UDPMulticastListener : IMulticastListener
    {
        private DatagramSocket socket;

        public bool IsRunning { get; private set; }
        public string MulticastIP { get; private set; }
        public int MulticastPort { get; private set; }

        public event UDPDataReceivedHandler DataReceived;
        protected void OnDataReceived(DataReceivedEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(this, e);
        }

        public async Task<bool> Startup(string multicastIP, int multicastPort)
        {
            Shutdown();
            IsRunning = true;

            this.MulticastIP = multicastIP;
            this.MulticastPort = multicastPort;

            try
            {
                socket = new DatagramSocket();
                socket.MessageReceived += socket_MessageReceived;
                await socket.BindEndpointAsync(null, multicastPort.ToString());
                socket.JoinMulticastGroup(new HostName(multicastIP));
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0} occurred while starting multicast listener: {1}", ex.GetType().Name, ex.Message));
                Shutdown();
                return false;
            }
        }

        public void Shutdown()
        {
            IsRunning = false;

            if (socket != null)
            {
                socket.MessageReceived -= socket_MessageReceived;
                socket.Dispose();
                socket = null;
            }
        }

        private async void socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            if (!IsRunning)
                return;

            MemoryStream stream = new MemoryStream();
            await args.GetDataStream().AsStreamForRead().CopyToAsync(stream);
            byte[] data = stream.ToArray();

            if (DataReceived != null)
            {
                OnDataReceived(new DataReceivedEventArgs()
                {
                    Data = data,
                    Length = data.Length,
                    SenderAddress = args.RemoteAddress.RawName
                });
            }
        }
    }
}
