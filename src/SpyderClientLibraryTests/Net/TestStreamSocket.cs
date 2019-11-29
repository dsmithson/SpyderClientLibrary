using Knightware.Net.Sockets;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    public class TestStreamSocket : IStreamSocket
    {
        private Socket socket;

        public bool IsConnected
        {
            get { return socket != null && socket.Connected; }
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

        public async Task<bool> StartupAsync(string serverIP, int serverPort)
        {
            await ShutdownAsync();

            IsRunning = true;
            this.ServerIP = serverIP;
            this.ServerPort = serverPort;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse(serverIP), serverPort));
                return true;
            }
            catch
            {
                await ShutdownAsync();
                return false;
            }
        }

        public Task ShutdownAsync()
        {
            IsRunning = false;

            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            return Task.FromResult(true);
        }


        public Task<int> ReadAsync(byte[] buffer, int offset, int length)
        {
            return Task.FromResult(socket.Receive(buffer, offset, length, SocketFlags.None));
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int length, int timeout)
        {
            socket.ReceiveTimeout = timeout;

            return Task.Run(() =>
                {
                    try
                    {

                        return socket.Receive(buffer, offset, length, SocketFlags.None);
                    }
                    catch (SocketException)
                    {
                        return 0;
                    }
                });
        }

        public Task WriteAsync(byte[] buffer, int offset, int length)
        {
            socket.Send(buffer, offset, length, SocketFlags.None);
            return Task.FromResult(true);
        }

        public Task WriteAsync(byte[] buffer, int offset, int length, int timeout)
        {
            socket.SendTimeout = timeout;
            return Task.Run(() =>
                socket.Send(buffer, offset, length, SocketFlags.None));
        }
    }
}
