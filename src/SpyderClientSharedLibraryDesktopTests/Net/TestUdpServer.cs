using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vista.SystemManager;

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
            this.ResponseData = response.Split(' ').ToList();
        }

        public TestUdpResponse(List<string> responseParts, ServerOperationResultCode result = ServerOperationResultCode.Success)
        {
            this.Result = result;
            this.ResponseData = responseParts.Select(r => r.Replace(" ", "%20")).ToList();
        }

        public TestUdpResponse(params object[] responseParts)
        {
            this.Result = ServerOperationResultCode.Success;
            this.ResponseData = responseParts?.Select(part => part.ToString().Replace(" ", "%20")).ToList();
        }
    }

    public class TestUdpServer : IDisposable
    {
        private Socket udpServer;
        private static SystemMgr sys;
        private static StringCommandProcessor stringProcessor;

        public Func<TestUdpCommand, TestUdpResponse> ProcessCommand { get; set; }

        public TestUdpServer()
        {
            //Load a test configuration
            LoadManifestFile("Spyder.Client.Resources.TestConfigs.Version4.SystemSettings.xml", @"c:\spyder\SystemSettings.xml");
            LoadManifestFile("Spyder.Client.Resources.TestConfigs.Version4.SystemConfiguration.xml", @"c:\spyder\SystemConfiguration.xml");
            LoadManifestFile("Spyder.Client.Resources.TestConfigs.Version4.Scripts.xml", @"c:\spyder\scripts\scripts.xml");

            //Load a systemMgr to process requests
            sys = new SystemMgr(@"c:\spyder\systemsettings.xml");
            sys.SystemDataFileName = @"c:\spyder\SystemConfiguration.xml";
            sys.Post();
            sys.AppInit();

            // udp interface
            stringProcessor = new StringCommandProcessor();
            stringProcessor.Startup(sys);

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

            if(stringProcessor != null)
            {
                stringProcessor.Shutdown();
                stringProcessor = null;
            }

            if(sys != null)
            {
                sys.Shutdown();
                sys = null;
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
                buffer[6] != (byte)0x00 ||
                buffer[7] != (byte)0x00 ||
                buffer[8] != (byte)0x00 ||
                buffer[9] != (byte)0x00)
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
                    //Use internal parser
                    stringProcessor.ProcessCommand(fullCommand, out fullResponse);
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
