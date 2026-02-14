using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Spyder.Client.Net
{
    public class TestUdpCommand
    {
        public string Command { get; set; }
        public string[] Args { get; set; }
    }

    public class TestUdpResponse : ServerOperationResult
    {
        public TestUdpResponse(string response, ServerOperationResultCode result = ServerOperationResultCode.Success)
        {
            this.Result = result;
            this.ResponseRaw = response;
        }

        public TestUdpResponse(List<string> responseParts, ServerOperationResultCode result = ServerOperationResultCode.Success)
        {
            this.Result = result;
            this.ResponseRaw = string.Join(" ", responseParts.Select(r => r.Replace(" ", "%20")).ToList());
        }

        public TestUdpResponse(params object[] responseParts)
        {
            this.Result = ServerOperationResultCode.Success;
            this.ResponseRaw = string.Join(" ", responseParts?.Select(part => part.ToString().Replace(" ", "%20")).ToList());
        }

        public TestUdpResponse(string responseRaw)
        {
            this.Result = ServerOperationResultCode.Success;
            this.ResponseRaw = responseRaw;
        }
    }

    public class TestUdpServer : IDisposable
    {
        private Socket udpServer;

        public Func<TestUdpCommand, TestUdpResponse> ProcessCommand { get; set; }

        public TestUdpServer()
        {
            udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpServer.Bind(new IPEndPoint(IPAddress.Loopback, SpyderUdpClient.ServerPort));
            BeginListening(new byte[1400]);
        }

        private void LoadManifestFile(string manifestFile, string localFile)
        {
            string directory = Path.GetDirectoryName(localFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var stream = File.Create(localFile))
            {
                var manifestStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestFile);
                manifestStream.CopyTo(stream);
            }
        }

        public void Dispose()
        {
            if (udpServer != null)
            {
                udpServer.Dispose();
                udpServer = null;
            }
        }

        private void BeginListening(byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            udpServer.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP, OnMessageReceived, buffer);
        }

        private void OnMessageReceived(IAsyncResult ar)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            int bytesReceived = udpServer.EndReceiveFrom(ar, ref remoteEP);
            if (bytesReceived <= 0)
                return;

            byte[] buffer = (byte[])ar.AsyncState;

            //Check header
            string fullResponse = "";
            if (bytesReceived < 10 ||
                buffer[0] != (byte)'s' ||
                buffer[1] != (byte)'p' ||
                buffer[2] != (byte)'y' ||
                buffer[3] != (byte)'d' ||
                buffer[4] != (byte)'e' ||
                buffer[5] != (byte)'r' ||
                buffer[6] != 0x00 ||
                buffer[7] != 0x00 ||
                buffer[8] != 0x00 ||
                buffer[9] != 0x00)
            {
                //Invalid header
                fullResponse = ((int)ServerOperationResultCode.ExecutionError).ToString();
            }
            else
            {
                //Parse command
                string fullCommand = ASCIIEncoding.ASCII.GetString(buffer, 10, buffer.Length - 10).TrimEnd();
                var commandParts = fullCommand.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //Process command and get a response
                if (ProcessCommand == null)
                {
                    throw new NotImplementedException("No handler was provided for processing the UDP message");
                }
                else
                {
                    var response = ProcessCommand(new TestUdpCommand()
                    {
                        Command = commandParts[0],
                        Args = commandParts.Skip(1).Select(arg => arg.Replace("%20", " ")).ToArray()
                    });
                    fullResponse = ((int)response.Result).ToString() + " " + string.Join(" ", response.ResponseData.Select(r => r.Replace(" ", "%20")));
                }
            }

            //Send response to caller
            byte[] fullResponseBytes = ASCIIEncoding.ASCII.GetBytes(fullResponse);
            udpServer.SendTo(fullResponseBytes, remoteEP);

            BeginListening(buffer);
        }
    }
}
