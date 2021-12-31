using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using Knightware.Diagnostics;
using System.Threading;
using Spyder.Client.Common;
using System.Globalization;
using Knightware.Primitives;

namespace Spyder.Client.Net
{
    public enum StillServerDisplayMode { NormalRGB, NormalAlpha, StripeRGB, StripeAlpha }

    public class StillServerClient
    {
        public const int DefaultServerPort = 9927;

        private Color? lastColorKey;
        private Socket socket;
        private IPAddress serverIP;
        private int serverPort;
        private string serverImagePath;
        private QFTClient qft;
        private Timer pingTimer;

        public bool Running { get; private set; }

        /// <summary>
        /// When not specified in calls, this controls the display index commands are being sent to.  If null (the default), no display will be specified in commands sent to the still server.
        /// </summary>
        public int? DefaultDisplayIndex { get; set; }

        public bool Connected
        {
            get
            {
                if (socket == null || !socket.Connected)
                    return false;
                else
                    return true;
            }
        }

        public async Task<bool> StartupAsync(string serverIP, int serverPort = DefaultServerPort)
        {
            await ShutdownAsync(false).ConfigureAwait(false);
            Running = true;

            try
            {
                if (!IPAddress.TryParse(serverIP, out this.serverIP))
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Invalid image server IP address specified");
                    await ShutdownAsync(true).ConfigureAwait(false);
                    return false;
                }
                this.serverPort = serverPort;

                //If the still server isn't available, or isn't running on the host machine for some reason,
                //we don't want to get stuck for a long time waiting for the socket to connect.  The async connect
                //call below will allow us to kill the socket connect if it takes longer than a reasonable amount of time.

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);
                IAsyncResult ar = socket.BeginConnect(serverIP, serverPort, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(3000))
                {
                    //Failed to connect
                    await ShutdownAsync(false).ConfigureAwait(false);
                    return false;
                }

                //Start up pinging
                this.pingTimer = new Timer(pingCallback, null, 60000, 30000);

                //Send an empty command (return character) to initialize the session
                System.Threading.Thread.Sleep(100);
                string rxData = await RetrieveAsync("\r\n", false).ConfigureAwait(false);
                if (rxData == null)
                {
                    await ShutdownAsync(false).ConfigureAwait(false);
                    return false;
                }

                //Get server image folder
                serverImagePath = await GetServerImagePathAsync().ConfigureAwait(false);
                if (string.IsNullOrEmpty(serverImagePath))
                {
                    await ShutdownAsync(false).ConfigureAwait(false);
                    return false;
                }

                //Startup QFT
                qft = new QFTClient(this.serverIP.ToString());
                await qft.StartupAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Exception starting up: " + ex.Message);
                return false;
            }
        }

        public async Task ShutdownAsync()
        {
            await ShutdownAsync(true)
                .ConfigureAwait(false);
        }
        protected async Task ShutdownAsync(bool complete)
        {
            if (complete)
                Running = false;

            lastColorKey = null;

            if (complete && this.pingTimer != null)
            {
                this.pingTimer.Dispose();
                this.pingTimer = null;
            }

            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
            if (qft != null)
            {
                await qft.ShutdownAsync().ConfigureAwait(false);
                qft = null;
            }
        }

        private string GetDisplayIndexCommandModifier(int? displayIndex)
        {
            if(displayIndex.HasValue)
            {
                return $"[{displayIndex.Value}] ";
            }
            else if(DefaultDisplayIndex.HasValue)
            {
                return $"[{DefaultDisplayIndex.Value}] ";
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<bool> SetMultiAsync(StillServerDisplayMode mode, int width, int height, string fileName, bool maintainAR = true, int? displayIndex = null)
        {
            if (!await EnsureFileAtServerAsync(fileName).ConfigureAwait(false))
                return false;

            string modeName = GetModeName(mode);
            if (string.IsNullOrEmpty(modeName))
                return false;

            string encodedName = Path.GetFileName(fileName).Replace(" ", "%20");
            string displayPrefix = GetDisplayIndexCommandModifier(displayIndex);
            string cmd = $"{displayPrefix}set mode {modeName} size {width} {height} content image {encodedName} maintainar {maintainAR}".ToLower();
            return await TransmitAsync(cmd).ConfigureAwait(false);
        }

        public async Task<bool> SetModeAsync(StillServerDisplayMode mode, int? displayIndex = null)
        {
            string modeName = GetModeName(mode);
            if (string.IsNullOrEmpty(modeName))
                return false;
            else
            {
                string displayPrefix = GetDisplayIndexCommandModifier(displayIndex);
                return await TransmitAsync($"{displayPrefix}set mode {modeName}").ConfigureAwait(false);
            }
        }

        private void pingCallback(object state)
        {
            Task t = RetrieveAsync("\r\n");
        }

        private string GetModeName(StillServerDisplayMode mode)
        {
            if (mode == StillServerDisplayMode.NormalAlpha)
                return "alpha";
            else if (mode == StillServerDisplayMode.NormalRGB)
                return "normal";
            else if (mode == StillServerDisplayMode.StripeAlpha)
                return "stripeAlpha";
            else if (mode == StillServerDisplayMode.StripeRGB)
                return "stripe";
            else
                return string.Empty;
        }

        public Color? GetLastColorKey()
        {
            return lastColorKey;
        }

        public async Task<bool> SetStripePageAsync(int page, int? displayIndex = null)
        {
            string displayPrefix = GetDisplayIndexCommandModifier(displayIndex);
            return await TransmitAsync($"{displayPrefix}set stripepage {page}")
                .ConfigureAwait(false);
        }

        public async Task<int> GetStripePageCountAsync(int? displayIndex = null)
        {
            string displayPrefix = GetDisplayIndexCommandModifier(displayIndex);
            string response = await RetrieveAsync($"{displayPrefix}get stripepagecount").ConfigureAwait(false);
            if (response != null)
            {
                response = response.Replace(">", string.Empty);
                if (int.TryParse(response, out int count))
                    return count;
            }
            return -1;
        }

        public async Task<bool> SetSizeAsync(int width, int height, int? displayIndex = null)
        {
            string displayPrefix = GetDisplayIndexCommandModifier(displayIndex);
            return await TransmitAsync($"{displayPrefix}set size {width} {height}")
                .ConfigureAwait(false);
        }

        private async Task<bool> EnsureFileAtServerAsync(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    return false;

                if (!qft.IsConnected)
                {
                    if (!await qft.StartupAsync())
                    {
                        TraceQueue.Trace(this, TracingLevel.Warning, "Failed to start QFT client session");
                        return false;
                    }
                }

                string targetFile = Path.Combine(serverImagePath, Path.GetFileName(fileName));
                targetFile = qft.ConvertAbsolutePathToRelative(targetFile);
                if (!await qft.FileExists(targetFile) || await qft.GetModifiedTime(targetFile) != new FileInfo(fileName).LastWriteTime)
                {
                    //Push the file to the still server
                    using var fileStream = File.OpenRead(fileName);
                    if (!await qft.SendFile(fileStream, targetFile, null).ConfigureAwait(false))
                    {
                        string rxData = await RetrieveAsync("\r\n", false).ConfigureAwait(false);
                        TraceQueue.Trace(this, TracingLevel.Warning, "Failed to write file to server");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Exception sending image server file: " + ex.Message);
                await qft.ShutdownAsync();
                return false;
            }
        }

        public async Task<bool> WriteImageAsync(string fileName, int? displayIndex = null)
        {
            if (await EnsureFileAtServerAsync(fileName).ConfigureAwait(false))
            {
                //Now send command to load file
                string displayPrefix = GetDisplayIndexCommandModifier(displayIndex);
                string encodedFile = Path.GetFileName(fileName).Replace(" ", "%20");
                return await TransmitAsync($"{displayPrefix}set content image {encodedFile}")
                    .ConfigureAwait(false);
            }
            else
            {
                //Failed to transmit file, so quit now
                return false;
            }
        }

        protected async Task<string> GetServerImagePathAsync()
        {
            string response = await RetrieveAsync("get imagepath").ConfigureAwait(false);
            if (response != null && response.Contains(">"))
            { 
                return response.Substring(0, response.IndexOf('>'));
            }
            return null;
        }

        protected async Task<bool> TransmitAsync(string command)
        {
            string response = await RetrieveAsync(command).ConfigureAwait(false);
            if (response == null)
                return false;

            //TODO:  Check result for success response
            return response.Contains("0>");
        }

        protected async Task<string> RetrieveAsync(string command)
        {
            string response = await RetrieveAsync(command, true).ConfigureAwait(false);
            if (response != null)
            {
                //If this command affected the image (set command), a 'last pixel' value will be returned that can be used
                //to verify when the entire image has actually been ingested by our frame buffer.
                int parseIndex = response.IndexOf("KEY=");
                if (parseIndex >= 0 && response.Length >= parseIndex + 15)
                {
                    //Value will be in the format: KEY=RRR,GGG,BBB
                    byte r = byte.Parse(response.Substring(parseIndex + 4, 3));
                    byte g = byte.Parse(response.Substring(parseIndex + 8, 3));
                    byte b = byte.Parse(response.Substring(parseIndex + 12, 3));
                    lastColorKey = Color.FromRgb(r, g, b);
                }
            }
            return response;
        }

        private async Task<string> RetrieveAsync(string command, bool attemptRecover)
        {
            if (!Running)
                return null;

            //Ensure connection
            if (!Connected)
            {
                if (attemptRecover)
                {
                    //Object has been initialized, but the connection is broken.  
                    //Try to restart one time
                    if (await StartupAsync(serverIP.ToString(), serverPort).ConfigureAwait(false))
                        return await RetrieveAsync(command, false).ConfigureAwait(false);
                }
                return null;
            }

            try
            {
                //Clear any previous data
                while (socket.Available > 0)
                {
                    byte[] readBuffer = new byte[socket.Available];
                    socket.Receive(readBuffer);
                }

                //Add return to end if not present
                if (!command.EndsWith("\r\n"))
                    command += "\r\n";

                //Write data and get response synchronously
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(command);
                socket.Send(bytes);

                byte[] rxBuffer = new byte[2048];
                int rxCount = socket.Receive(rxBuffer);
                if (rxCount <= 0)
                    return null;

                string response = ASCIIEncoding.ASCII.GetString(rxBuffer, 0, rxCount);
                response = response.Replace("\r", string.Empty).Replace("\n", string.Empty);
                return response;
            }
            catch (Exception ex)
            {
                if (attemptRecover)
                {
                    //Try to restart / re-connect and try again
                    if (await StartupAsync(serverIP.ToString(), serverPort).ConfigureAwait(false))
                        return await RetrieveAsync(command, false).ConfigureAwait(false);
                    else
                        return null;
                }
                else
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Exception sending command to Image server: " + ex.Message);
                    return null;
                }
            }
        }
    }
}
