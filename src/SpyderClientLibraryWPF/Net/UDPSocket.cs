using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Diagnostics;
using Spyder.Client.Net.Sockets;

namespace Spyder.Client.Net
{
    public class UDPSocket : IUDPSocket
    {
        private Stack<TaskCompletionSource<byte[]>> messageReceiptAwaiters;
        private Socket socket;
        private IPAddress server;

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

        public UDPSocket()
        {
        }

        public Task<bool> Startup(string serverIP, int serverPort)
        {
            Shutdown();
            this.IsRunning = true;
            this.ServerIP = serverIP;
            this.ServerPort = serverPort;

            if (string.IsNullOrEmpty(serverIP) || !IPAddress.TryParse(serverIP, out server))
            {
                Shutdown();
                return Task.FromResult(false);
            }
            
            messageReceiptAwaiters = new Stack<TaskCompletionSource<byte[]>>();
            
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            if (server.Equals(IPAddress.Broadcast))
            {
                socket.EnableBroadcast = true;
            }
            
            if (!BeginReceive())
            {
                Shutdown();
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public void Shutdown()
        {
            IsRunning = false;

            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }

            messageReceiptAwaiters = null;
        }

        private bool BeginReceive()
        {
            if (socket == null || !IsRunning)
                return false;

            byte[] buffer = new byte[1500];
            EndPoint remoteEP = new IPEndPoint(server, ServerPort);
            socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP, socket_DataReceived, buffer);
            return true;
        }

        void socket_DataReceived(IAsyncResult ar)
        {
            if (!IsRunning || socket == null)
                return;

            try
            {
                byte[] rxBuffer = (byte[])ar.AsyncState;
                EndPoint remoteEP = new IPEndPoint(server, ServerPort);
                int count = socket.EndReceiveFrom(ar, ref remoteEP);
                if (count <= 0)
                    return;

                TaskCompletionSource<byte[]> tcs = null;
                lock (messageReceiptAwaiters)
                {
                    if (messageReceiptAwaiters.Count > 0)
                    {
                        tcs = messageReceiptAwaiters.Pop();
                    }
                }

                if (tcs != null)
                {
                    byte[] buffer = new byte[count];
                    Array.Copy(rxBuffer, 0, buffer, 0, buffer.Length);
                    tcs.TrySetResult(buffer);
                }
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while receiving UDP data: {1}", ex.GetType().Name, ex.Message);
            }
            finally
            {
                BeginReceive();
            }
        }

        public Task<bool> SendDataAsync(byte[] buffer, int startIndex, int length)
        {
            if (!IsRunning || socket == null)
                return Task.FromResult(false);

            var tcs = new TaskCompletionSource<bool>();
            var args = new SocketAsyncEventArgs()
            {
                UserToken = tcs,
                RemoteEndPoint = new IPEndPoint(server, ServerPort)
            };
            args.SetBuffer(buffer, startIndex, length);
            args.Completed += socket_SendCompleted;
            if (!socket.SendToAsync(args))
            {
                //Completed synchronously
                socket_SendCompleted(socket, args);
            }

            return tcs.Task;
        }

        void socket_SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (!IsRunning || socket == null)
                return;

            bool success = (e.SocketError == SocketError.Success);
            var tcs = (TaskCompletionSource<bool>)e.UserToken;
            tcs.TrySetResult(success);
        }

        public async Task<byte[]> RetrieveDataAsync(byte[] txBuffer, int startIndex, int length, TimeSpan timeout)
        {
            //Queue for receipt of message immediately
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();
            lock (messageReceiptAwaiters)
            {
                messageReceiptAwaiters.Push(tcs);
            }

            //Try to send our data
            if (!await SendDataAsync(txBuffer, startIndex, length))
                return null;

            //Wait for response
            Task timeoutTask = Task.Delay(timeout);
            await Task.WhenAny(timeoutTask, tcs.Task);

            //Did we get a response?
            if (tcs.Task.Exception == null && tcs.Task.Status == TaskStatus.RanToCompletion)
                return tcs.Task.Result;
            else
                return null;
        }
    }
}
