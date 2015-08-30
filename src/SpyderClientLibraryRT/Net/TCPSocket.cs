using Knightware.Net.Sockets;
using Knightware.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Spyder.Client.Net
{
    public class TCPSocket : IStreamSocket
    {
        private StreamSocket socket;
        private SocketStream stream;

        public bool IsRunning { get; private set; }

        public string ServerIP { get; private set; }

        public int ServerPort { get; private set; }

        public bool IsConnected
        {
            get
            {
                if (socket != null && stream != null && stream.CanRead && stream.CanWrite)
                    return true;
                else
                    return false;
            }
        }
        
        public async Task<bool> Startup(string serverIP, int serverPort)
        {
            Shutdown();
            IsRunning = true;

            this.ServerIP = serverIP;
            this.ServerPort = serverPort;

            try
            {
                return await EnsureConnected();
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(TracingLevel.Error, "{0} occurred while starting TcpSocket: {1}", ex.GetType().Name, ex.Message);
                Shutdown();
                return false;
            }
        }

        public void Shutdown()
        {
            IsRunning = false;

            if (stream != null)
            {
                stream = null;
            }

            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }
        
        public Task<int> ReadAsync(byte[] buffer, int offset, int length)
        {
            return DoSocketStreamOperation((s) => s.ReadAsync(buffer, offset, length));
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int length, int timeout)
        {
            var tokenSource = new CancellationTokenSource(timeout);
            return DoSocketStreamOperation((s) => s.ReadAsync(buffer, offset, length, tokenSource.Token));
        }

        public Task WriteAsync(byte[] buffer, int offset, int length)
        {
            return DoSocketStreamOperation((s) => s.WriteAsync(buffer, offset, length));
        }

        public Task WriteAsync(byte[] buffer, int offset, int length, int timeout)
        {
            var tokenSource = new CancellationTokenSource(timeout);
            return DoSocketStreamOperation((s) => s.WriteAsync(buffer, offset, length, tokenSource.Token));
        }

        private Task DoSocketStreamOperation(Func<SocketStream, Task> operation)
        {
            return DoSocketStreamOperation(async (s) =>
                {
                    await operation(s);
                    return true;
                });
        }

        private async Task<T> DoSocketStreamOperation<T>(Func<SocketStream, Task<T>> operation)

        {
            try
            {
                if (!await EnsureConnected())
                    return default(T);


                return await operation(stream);
            }
            catch (TaskCanceledException)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Timed out waiting for socket operation completion");
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while performing socket operation: {1}", ex.GetType().Name, ex.Message);
            }

            TearDownSocket();
            return default(T);
        }

        private async Task<bool> EnsureConnected()
        {
            if (!IsRunning)
            {
                return false;
            }
            else if (IsConnected)
            {
                return true;
            }
            else
            {
                TearDownSocket();

                try
                {
                    socket = new StreamSocket();
                    await socket.ConnectAsync(new EndpointPair(null, string.Empty, new HostName(ServerIP), ServerPort.ToString()));

                    stream = new SocketStream(socket.InputStream, socket.OutputStream);
                    return true;
                }
                catch (Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while starting socket: {1}", ex.GetType().Name, ex.Message);
                    TearDownSocket();
                    return false;
                }
            }
        }

        private void TearDownSocket()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }

            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }
    }
}
