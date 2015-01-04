using Spyder.Client.Net.Sockets;
using Spyder.Client.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    public class TCPSocket : IStreamSocket
    {
        private TcpClient client;
        private NetworkStream stream;

        public bool IsConnected
        {
            get { return client != null && client.Connected; }
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int length)
        {
            if (!IsRunning)
                return Task.FromResult(-1);

            return stream.ReadAsync(buffer, offset, length);
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int length, int timeout)
        {
            if (!IsRunning)
                return Task.FromResult(-1);

            return stream.ReadAsync(buffer, offset, length, new CancellationTokenSource(timeout).Token);
        }

        public Task WriteAsync(byte[] buffer, int offset, int length)
        {
            if (!IsRunning)
                return Task.FromResult(false);

            return stream.WriteAsync(buffer, offset, length);
        }

        public Task WriteAsync(byte[] buffer, int offset, int length, int timeout)
        {
            if (!IsRunning)
                return Task.FromResult(false);

            return stream.WriteAsync(buffer, offset, length, new CancellationTokenSource(timeout).Token);
        }

        public bool IsRunning
        {
            get;
            private set;
        }

        public string ServerIP
        {
            get;
            private set;
        }

        public int ServerPort
        {
            get;
            private set;
        }

        public async Task<bool> Startup(string serverIP, int serverPort)
        {
            Shutdown();
            IsRunning = true;

            this.ServerIP = serverIP;
            this.ServerPort = serverPort;

            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverIP, serverPort);
                stream = client.GetStream();
                return true;
            }
            catch(Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while starting up socket: {1}", ex.GetType().Name, ex.Message);
                Shutdown();
                return false;
            }
        }

        public void Shutdown()
        {
            IsRunning = false;

            if(client != null)
            {
                client.Close();
                client = null;
            }

            if(stream != null)
            {
                stream.Dispose();
                stream = null;
            }

            this.ServerIP = null;
            this.ServerPort = 0;
        }
    }
}
