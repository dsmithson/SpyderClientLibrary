using Knightware.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace Spyder.Client.Net
{
    public class UDPSocket : IUDPSocket
    {
        private Stack<TaskCompletionSource<byte[]>> messageReceiptAwaiters;
        private DatagramSocket socket;

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

        public async Task<bool> Startup(string serverIP, int serverPort)
        {
            Shutdown();
            IsRunning = true;

            messageReceiptAwaiters = new Stack<TaskCompletionSource<byte[]>>();

            socket = new DatagramSocket();            
            socket.MessageReceived += socket_MessageReceived;
            await socket.ConnectAsync(new HostName(serverIP), serverPort.ToString());
            
            return true;
        }

        public void Shutdown()
        {
            IsRunning = false;

            //Wait for incoming messages to finish being processed
            DateTime timeoutTime = DateTime.Now.AddSeconds(5);
            while(messageReceiptAwaiters != null && messageReceiptAwaiters.Count > 0 && DateTime.Now < timeoutTime)
            {
                Task.Delay(100).Wait();
            }
            messageReceiptAwaiters = null;
            
            if (socket != null)
            {
                try
                {
                    socket.MessageReceived -= socket_MessageReceived;
                }
                catch(Exception ex)
                {
                    //HACK:  In testing I see the MessageReceived unsubscription throwing an InvalidOperationException saying 'A method was called at an unexpected time.'
                    Debug.WriteLine(string.Format("{0} occurred while shutting down socket: {1}", ex.GetType().Name, ex.Message));
                }
                socket.Dispose();
                socket = null;
            }
        }

        void socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
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
                var reader = args.GetDataReader();
                byte[] buffer = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(buffer);
                tcs.TrySetResult(buffer);
            }
        }

        public async Task<bool> SendDataAsync(byte[] buffer, int startIndex, int length)
        {
            if (!IsRunning)
                return false;

            using (DataWriter writer = new DataWriter(socket.OutputStream))
            {
                writer.WriteBytes(buffer);
                //await writer.FlushAsync();
                await writer.StoreAsync();

                writer.DetachStream();
            }
            return true;
        }

        public async Task<byte[]> RetrieveDataAsync(byte[] txBuffer, int startIndex, int length, TimeSpan timeout)
        {
            if (!IsRunning)
                return null;

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
