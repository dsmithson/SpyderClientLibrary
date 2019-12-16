using Knightware.Diagnostics;
using Knightware.Net;
using Knightware.Net.Sockets;
using Knightware.Threading.Tasks;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    public delegate void FileProgressDelegate(long bytes, long totalBytes, string fileName);

    /// <summary>
    /// Supports the client side Spyder Quick File Transfer (QFT) protocol for handling file transfers
    /// </summary>
    public class QFTClient
    {
        public const int StreamBlockSize = 8192;
        public const int RECEIVE_TIMEOUT = 5000;
        public const int SERVER_PORT = 7280;
        public const int InactivityDisconnectSeconds = 15;

        private IStreamSocket socket;
        private AutoResetWorker pingWorker;
        private DateTime lastCommandTime;
        private readonly AsyncLock commandLock = new AsyncLock();

        public bool IsRunning { get; private set; }

        public string ServerIP { get; private set; }
        public int ServerPort { get; private set; }

        public bool IsConnected
        {
            get
            {
                if (IsRunning && socket != null && socket.IsConnected)
                    return true;
                else
                    return false;
            }
        }

        public bool FileCompressionEnabled { get; private set; }

        public QFTClient(string serverIP, int serverPort = 7280)
        {
            this.ServerIP = serverIP;
            this.ServerPort = serverPort;
        }

        public Task<bool> StartupAsync()
        {
            return StartupAsync(true, true, true);
        }

        protected async Task<bool> StartupAsync(bool sendRemoteDisconnectRequest, bool acquireSemaphore, bool enableCompression)
        {
            await ShutdownAsync(sendRemoteDisconnectRequest, acquireSemaphore);
            IsRunning = true;

            try
            {
                socket = new TCPSocket();
                if (!await socket.StartupAsync(ServerIP, ServerPort))
                {
                    TraceQueue.Trace(TracingLevel.Warning, "Failed to initialize socket for QFT Client. Shutting down...");
                    await ShutdownAsync();
                    return false;
                }

                //try to enable compression for session
                if (enableCompression)
                {
                    if (!await EnableCompressionForCurrentSession(acquireSemaphore))
                    {
                        //Failed to enable compression; startup without compression
                        return await StartupAsync(sendRemoteDisconnectRequest, acquireSemaphore, false);
                    }
                }

                lastCommandTime = DateTime.Now;
                pingWorker = new AutoResetWorker();
                pingWorker.PeriodicSignallingTime = TimeSpan.FromSeconds(10);
                await pingWorker.StartupAsync(PingWorker_DoWork, null, () => IsRunning);

                return true;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(TracingLevel.Error, "{0} occurred while initializing socket: {1}", ex.GetType().Name, ex.Message);
            }

            await ShutdownAsync();
            return false;
        }

        public Task ShutdownAsync()
        {
            return ShutdownAsync(true, true);
        }

        private async Task ShutdownAsync(bool sendRemoteDisconnectRequest, bool acquireSemaphore)
        {
            AsyncLock.Releaser? releaser = null;
            try
            {
                if (pingWorker != null)
                {
                    await pingWorker.ShutdownAsync();
                    pingWorker = null;
                }

                if (acquireSemaphore)
                {
                    releaser = await commandLock.LockAsync();
                }

                if (IsConnected && sendRemoteDisconnectRequest)
                {
                    await requestServerDisconnect();
                }

                IsRunning = false;

                if (socket != null)
                {
                    await socket.ShutdownAsync();
                    socket = null;
                }

                FileCompressionEnabled = false;
            }
            catch (Exception ex)
            {
                Log(ex, "Shutdown");
            }
            finally
            {
                if (releaser.HasValue)
                    releaser.Value.Dispose();
            }
        }

        #region Helper Utilities

        /// <summary>
        /// Converts an absolute (c:\...) filename to  a root relative name for QFT
        /// </summary>
        /// <param name="absoluteFile"></param>
        /// <returns></returns>
        public string ConvertAbsolutePathToRelative(string absoluteFile)
        {
            return QFTClient.ConvertToRelativePath(absoluteFile);
        }

        public static string ConvertToRelativePath(string absoluteFile)
        {
            //check for the presence of a drive letter at the beginning of the path.  May not be the 'C' drive.
            if (absoluteFile != null && absoluteFile.Length > 3 && absoluteFile.Substring(1, 2).ToLower() == ":\\")
                return absoluteFile.Substring(3, absoluteFile.Length - 3);
            else
                return string.Empty;
        }

        protected DateTime DateTimeFromOADate(double d)
        {
            return new DateTime(DoubleDateToTicks(d), DateTimeKind.Unspecified);
        }

        private long DoubleDateToTicks(double value)
        {
            if ((value >= 2958466.0) || (value <= -657435.0))
            {
                throw new ArgumentException("Ole Automation Date is Invalid");
            }

            long num = (long)((value * 86400000.0) + ((value >= 0.0) ? 0.5 : -0.5));
            if (num < 0L)
            {
                num -= (num % 0x5265c00L) * 2L;
            }
            num += 0x3680b5e1fc00L;
            if ((num < 0L) || (num >= 0x11efae44cb400L))
            {
                throw new ArgumentException("Ole Automation Date Scale Exception");
            }
            return (num * 0x2710L);
        }

        #endregion

        #region Internal Utility Methods

        /// <summary>
        /// Sends a command to the connected QFT Server and stores response into provided byte[] buffer.
        /// </summary>
        /// <param name="SendCommand">Command To Send</param>
        /// <param name="ReceiveBuffer">Buffer to receive command into</param>
        /// /// <param name="autoAttemptRecoverConnection">Determines whether or not to attemmpt to ensure connection and auto-reset the connection if connection check fails.</param>
        /// <returns>number of bytes received</returns>
        protected Task<int> ReceiveBytes(string SendCommand, ref byte[] ReceiveBuffer, bool autoAttemptRecoverConnection, int receiveTimeout = RECEIVE_TIMEOUT)
        {
            return ReceiveBytes(Encoding.UTF8.GetBytes(SendCommand), ReceiveBuffer, autoAttemptRecoverConnection, receiveTimeout);
        }

        /// <summary>
        /// Sends a command to the connected QFT Server and stores response into provided byte[] buffer.
        /// </summary>
        /// <param name="SendCommand">Command To Send, or set to string.Empty to receive only</param>
        /// <param name="ReceiveBuffer">Buffer to receive command into.  Setting to 0 bytes will cause method not to attempt receiving.</param>
        /// <param name="autoAttemptRecoverConnection">Determines whether or not to attemmpt to ensure connection and auto-reset the connection if connection check fails.</param>
        /// <returns>number of bytes received</returns>
        protected async Task<int> ReceiveBytes(byte[] SendCommand, byte[] ReceiveBuffer, bool autoAttemptRecoverConnection, int receiveTimeout = RECEIVE_TIMEOUT)
        {
            if (!IsRunning)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "QFT Client attempted a retrieve while instance was not running");
                return -1;
            }

            int read = 0;
            int ptr = 0;

            if (autoAttemptRecoverConnection)
            {
                //Send ping to verify connection
                if (!IsConnected || !await Ping(false))
                {
                    //Connection has been lost.  Attempt to recover
                    TraceQueue.Trace(this, TracingLevel.Information, "QFT connection to {0} is closed.  Attempting to open a new connection.", ServerIP);
                    if (await StartupAsync(false, false, true))
                    {
                        TraceQueue.Trace(this, TracingLevel.Information, "QFT connection to {0} has been re-opened.", ServerIP);
                    }
                    else
                    {
                        TraceQueue.Trace(this, TracingLevel.Warning, "Failed to open a new connection to {0}.", ServerIP);
                        await ShutdownAsync();
                        return -1;
                    }
                }
            }

            if (!IsConnected)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "QFT call failed because socket is not currently connected.");
                return -1;
            }

            try
            {
                //Send command
                if (SendCommand != null && SendCommand.Length > 0)
                {
                    //Send the command
                    await socket.WriteAsync(SendCommand, 0, SendCommand.Length);
                }

                //Read back a response
                if (ReceiveBuffer != null && ReceiveBuffer.Length > 0)
                {
                    try
                    {
                        //Receive response
                        do
                        {
                            read = await socket.ReadAsync(ReceiveBuffer, ptr, ReceiveBuffer.Length - ptr, receiveTimeout);
                            ptr += read;
                        }
                        while (read > 0 && ptr < ReceiveBuffer.Length);
                    }
                    catch (Exception ex)
                    {
                        TraceQueue.Trace(this, TracingLevel.Information, "Timeout in ReceiveBytes (QFT): " + ex.Message);
                    }
                }
                lastCommandTime = DateTime.Now;
                return ptr;
            }
            catch (Exception ex)
            {
                Log(ex, "ReceiveBytes");
            }

            //If we get here, we failed above
            await ShutdownAsync(false, false);
            return -1;
        }

        public async Task<bool> EnableCompressionForCurrentSession(bool acquireSemaphore)
        {
            const string command = "?CREG\r";
            const int size = 1;
            byte[] buffer = new byte[size];

            AsyncLock.Releaser? releaser = null;
            try
            {
                if (acquireSemaphore)
                    releaser = await commandLock.LockAsync();

                TraceQueue.Trace(this, TracingLevel.Information, "Attempting to enable file compression with server {0}", ServerIP);
                await ReceiveBytes(command, ref buffer, false);

                if (buffer[0] == 0x01)
                {
                    FileCompressionEnabled = true;
                    TraceQueue.Trace(this, TracingLevel.Information, "Compression for current session has been enabled");
                    return true;
                }
                else
                {
                    FileCompressionEnabled = false;
                    TraceQueue.Trace(this, TracingLevel.Information, "Unable to enable file compression for current session");
                    return false;
                }
            }
            catch (Exception ex)
            {
                FileCompressionEnabled = false;
                TraceQueue.Trace(this, TracingLevel.Warning, "QFTClient encountered {0} in compression request: {1}", ex.GetType().Name, ex.Message);
                return false;
            }
            finally
            {
                if (releaser.HasValue)
                    releaser.Value.Dispose();
            }
        }

        #endregion

        #region QFT Public File Functions

        public Task<bool> Ping()
        {
            return Ping(true);
        }

        /// <summary>
        /// Generic ping method, allows client to verify server connectivity
        /// </summary>
        /// <returns>true for successful response, false for failure</returns>
        protected async Task<bool> Ping(bool obtainSemaphore)
        {
            const string command = "?CREA\r";
            const int size = 1;
            byte[] buffer = new byte[size];

            AsyncLock.Releaser? releaser = null;
            try
            {
                if (obtainSemaphore)
                    releaser = await commandLock.LockAsync();

                TraceQueue.Trace(this, TracingLevel.Information, "Pinging server at {0}", ServerIP);
                await ReceiveBytes(command, ref buffer, false);

                if (buffer[0] == 0x01)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "QFTClient encountered an error in ping request: " + ex.Message);
                return false;
            }
            finally
            {
                if (releaser.HasValue)
                    releaser.Value.Dispose();
            }
        }

        /// <summary>
        /// Gets the difference between the client (local) time and the server
        /// </summary>
        /// <returns></returns>
        public async Task<TimeSpan> GetRemoteTimeOffset()
        {
            const string command = "?CRE5\r";
            const int size = 8;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Getting Remote time offset from {0}", ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return TimeSpan.MinValue;
                }

                //Parse remote time and send time skew
                DateTime remoteTime = DateTimeFromOADate(BitConverter.ToDouble(buffer, 0)).ToUniversalTime();
                remoteTime = remoteTime.ToLocalTime();
                TimeSpan response = DateTime.Now.Subtract(remoteTime);
                return response;
            }
            catch (Exception ex)
            {
                Log(ex, "GetRemoteTimeOffset");
                return TimeSpan.MinValue;
            }
        }


        /// <summary>
        /// Gets the Creation Time property on a specified file
        /// </summary>
        /// <param name="RemoteFile">File name to retrieve creation time for.  This is QFT server root relative.</param>
        /// <returns></returns>
        public async Task<DateTime> GetCreationTime(string RemoteFile)
        {
            string command = string.Format("?CRE8{0}\r", RemoteFile);
            const int size = 8;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Getting creation time for '{0}' on {1}", RemoteFile, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return DateTime.MinValue;
                }

                long fileTime = BitConverter.ToInt64(buffer, 0);
                DateTime response = DateTime.FromFileTimeUtc(fileTime);
                response = response.ToLocalTime();
                return response;
            }
            catch (Exception ex)
            {
                Log(ex, "GetCreationTime");
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Sets the modification time on a specified file
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="newTime">Local time to set remote file to.</param>
        public async Task<bool> SetModifiedTime(string remoteFile, DateTime newTime)
        {
            string command = string.Format("?CREE{0}:{1}\r", remoteFile, newTime.ToFileTimeUtc().ToString());
            const int size = 1;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Setting file modify time to {0} for '{1}' on {2}", newTime, remoteFile, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return false;
                }
                return (buffer[0] == 1 ? true : false);
            }
            catch (Exception ex)
            {
                Log(ex, "SetModifiedTime");
                return false;
            }
        }

        /// <summary>
        /// Gets the last Modification (write) Time property on a specified file
        /// </summary>
        /// <param name="remoteFile">File name to retrieve modify time for.  This is QFT server root relative.</param>
        /// <returns></returns>
        public async Task<DateTime> GetModifiedTime(string remoteFile)
        {
            string command = string.Format("?CRE9{0}\r", remoteFile);
            const int size = 8;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Getting last modified time for '{0}' on {1}", remoteFile, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return DateTime.MinValue;
                }
                long fileTime = BitConverter.ToInt64(buffer, 0);
                DateTime response = DateTime.FromFileTimeUtc(fileTime);

                response = response.ToLocalTime();
                return response;
            }
            catch (Exception ex)
            {
                Log(ex, "GetModifiedTime");
                return DateTime.MinValue;
            }
        }


        /// <summary>
        /// Gets a list of files in a specified directory
        /// </summary>
        /// <param name="directory">Server root relative directory to get file list from</param>
        /// <returns></returns>
        public async Task<string[]> GetFiles(string directory)
        {
            string command = string.Format("?CRE7{0}\r", directory);
            int totalLength = 0;
            byte[] buffer = new byte[StreamBlockSize];
            StringBuilder builder;
            int read;

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Getting file list in {0} on {1}", directory, ServerIP);
                    byte[] sizeBuffer = new byte[8];
                    if (await ReceiveBytes(command, ref sizeBuffer, true) <= 0)
                        return null;

                    for (int i = 0; i < 4; i++)
                        totalLength |= (sizeBuffer[i] << (i * 8));

                    //Any files?
                    if (totalLength <= 0)
                        return new string[0];

                    builder = new StringBuilder(totalLength);
                    do
                    {
                        if (totalLength < buffer.Length)
                            buffer = new byte[totalLength];

                        //Receive the data - we've already sent the command
                        read = await ReceiveBytes(string.Empty, ref buffer, false);
                        for (int i = 0; i < read; i++)
                            builder.Append((char)buffer[i]);

                        totalLength -= read;

                    } while (read > 0 && totalLength > 0);
                }
                return builder.ToString().Split('\\');
            }
            catch (Exception ex)
            {
                Log(ex, "GetFiles");
                return new string[0];
            }
        }

        /// <summary>
        /// Gets a list of files in a specified directory
        /// </summary>
        /// <param name="directory">Server root relative directory to get file list from</param>
        /// <returns></returns>
        public async Task<string[]> GetDirectories(string directory)
        {
            string command = string.Format("?CRE6{0}\r", directory);
            int totalLength = 0;
            byte[] buffer = new byte[StreamBlockSize];
            StringBuilder builder;
            int read;

            try
            {
                byte[] sizeBuffer = new byte[8];
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Getting directories in '{0}' on {1}", directory, ServerIP);
                    if (await ReceiveBytes(command, ref sizeBuffer, true) <= 0)
                        return null;

                    for (int i = 0; i < 4; i++)
                        totalLength |= (sizeBuffer[i] << (i * 8));

                    //Any Directories?
                    if (totalLength <= 0)
                        return new string[0];

                    builder = new StringBuilder(totalLength);
                    do
                    {
                        if (totalLength < buffer.Length)
                            buffer = new byte[totalLength];

                        //Receive the data - we've already sent the command
                        read = await ReceiveBytes(string.Empty, ref buffer, false);
                        for (int i = 0; i < read; i++)
                            builder.Append((char)buffer[i]);

                        totalLength -= read;

                    } while (read > 0 && totalLength > 0);
                }

                return builder.ToString().Split('\\');
            }
            catch (Exception ex)
            {
                Log(ex, "GetDirectories");
                return new string[0];
            }
        }

        /// <summary>
        /// Deletes a single file from the remote server
        /// </summary>
        /// <param name="remoteFile">Server root relative file path to delete</param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(string remoteFile)
        {
            string command = string.Format("?CRE3{0}\r", remoteFile);
            const int size = 1;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Deleting file '{0}' on {1}", remoteFile, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return false;
                }
                return (buffer[0] == 1 ? true : false);
            }
            catch (Exception ex)
            {
                Log(ex, "DeleteFile");
                return false;
            }
        }


        /// <summary>
        /// Deletes a directory recursively from the remote server
        /// </summary>
        /// <param name="remoteDirectory">Server root relative directory path to delete</param>
        /// <returns></returns>
        public async Task<bool> DeleteDirectory(string remoteDirectory)
        {
            string command = string.Format("?CRED{0}\r", remoteDirectory);
            const int size = 1;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Deleting directory '{0}' on {1}", remoteDirectory, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return false;
                }
                return (buffer[0] == 1 ? true : false);
            }
            catch (Exception ex)
            {
                Log(ex, "DeleteDirectory");
                return false;
            }
        }


        /// <summary>
        /// Creates a directory on the remote server
        /// </summary>
        /// <param name="remoteDirectory">Server root relative directory to create</param>
        /// <returns></returns>
        public async Task<bool> CreateDirectory(string remoteDirectory)
        {
            string command = string.Format("?CREC{0}\r", remoteDirectory);
            const int size = 1;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Creating directory '{0}' on {1}", remoteDirectory, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return false;
                }
                return (buffer[0] == 1 ? true : false);
            }
            catch (Exception ex)
            {
                Log(ex, "CreateDirectory");
                return false;
            }
        }

        /// <summary>
        /// Sends a local file to the remote server
        /// </summary>
        /// <param name="remoteFile">Remote file location to sent to (server root relative)</param>
        /// <returns>true for success, false for failure</returns>
        public async Task<bool> SendFile(Stream source, string remoteFile, FileProgressDelegate progress)
        {
            if (source == null || !source.CanRead)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Cannot process QFT Command SendFile, because supplied stream was null or not readable.");
                return false;
            }
            if (!IsRunning)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Cannot process QFT Command SendFile, because QFT client is not currently running.");
                return false;
            }

            Stream stream = source;
            bool disposeStream = false;
            string tempFileName = null;

            try
            {
                //Pre-compress the stream to a temp file if compression is enabled
                if (FileCompressionEnabled)
                {
                    //Create a new stream which will contain compressed data to be sent over the network
                    tempFileName = Path.GetTempFileName();

                    stream = File.Create(tempFileName);
                    disposeStream = true;

                    //Compress stream
                    using (DeflateStream compressor = new DeflateStream(stream, CompressionMode.Compress, true))
                    {
                        await source.CopyToAsync(compressor);
                        await compressor.FlushAsync();
                    }
                }

                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Sending file '{0}' to {1}", remoteFile, ServerIP);
                    long lengthRemaining = stream.Length;
                    byte[] command;
                    if (lengthRemaining.ToString().Length <= 8)
                    {
                        //The CRE1 command is the most widely supported command, however it will only work in cases where the 
                        //length of the file when represented as a string is eight characters or less.
                        string streamLength = lengthRemaining.ToString().PadLeft(8, '0');
                        command = Encoding.UTF8.GetBytes(string.Format("?CRE1{0}\r{1}", remoteFile, streamLength));
                    }
                    else
                    {
                        //The CREI command takes a full long value for a file size, so it can be used whenever the file size
                        //being transferred is longer than the eight ASCII character limit present in the CRE1 command.
                        command = Encoding.UTF8.GetBytes(string.Format("?CREI{0}\r00000000", remoteFile));
                        for (int i = 0; i < 8; i++)
                        {
                            byte currentByte = (byte)(lengthRemaining >> (i * 8));
                            int offset = command.Length - i - 1;
                            command[offset] = currentByte;
                        }
                    }

                    byte[] empty = new byte[0];
                    byte[] txBuffer = new byte[StreamBlockSize];
                    byte[] buffer = new byte[4];
                    long totalLength = stream.Length;
                    int count = (lengthRemaining > StreamBlockSize ? StreamBlockSize : (int)lengthRemaining);
                    bool cmdSent = false;

                    string name = Path.GetFileName(remoteFile);
                    stream.Seek(0, SeekOrigin.Begin);
                    lengthRemaining = stream.Length;

                    do
                    {
                        if (!cmdSent)
                        {
                            //First iteration of loop, so ensure the connection
                            await ReceiveBytes(command, empty, true);   //don't receive anything back yet
                            cmdSent = true;
                            continue;
                        }
                        else if (lengthRemaining > StreamBlockSize)
                        {
                            //don't receive anything back yet
                            await stream.ReadAsync(txBuffer, 0, count);
                            if (await ReceiveBytes(txBuffer, empty, false) == -1)
                            {
                                TraceQueue.Trace(this, TracingLevel.Warning, "Send failed to remote server");
                                return false;
                            }
                        }
                        else
                        {
                            int timeout = 1000 * 60 * 5;    //5 minutes
                            txBuffer = new byte[count];
                            await stream.ReadAsync(txBuffer, 0, count);
                            await ReceiveBytes(txBuffer, buffer, false, timeout);           //last transmission, so get acknowledgement back
                        }

                        lengthRemaining -= count;
                        if (lengthRemaining < count)
                            count = (int)lengthRemaining;

                        if (progress != null)
                            progress(totalLength - lengthRemaining, totalLength, "Sending " + name);

                    } while (count > 0);

                    if (buffer[0] == 'd' && buffer[1] == 'o' && buffer[2] == 'n' && buffer[3] == 'e')
                        return true;
                    else
                    {
                        TraceQueue.Trace(this, TracingLevel.Warning, "Failed to get confirmation of file transfer from remote server.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "QFTClient encountered an error sending file: " + ex.Message);
                return false;
            }
            finally
            {
                if (disposeStream)
                    stream.Dispose();

                try
                {
                    if (!string.IsNullOrWhiteSpace(tempFileName) && File.Exists(tempFileName))
                        File.Delete(tempFileName);
                }
                catch (Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred when trying to delete QFT temp file {1}: {2}", ex.GetType().Name, tempFileName, ex.Message);
                }
            }
        }

        /// <summary>
        /// Checks remote server for existance of a specified file
        /// </summary>
        /// <param name="remoteFile">File to check for.</param>
        /// <returns>True if file exists, false if not</returns>
        public async Task<bool> FileExists(string remoteFile)
        {
            string command = string.Format("?CRE4{0}\r", remoteFile);
            const int size = 1;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Checking if file '{0}' exists on {1}", remoteFile, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return false;
                }
                return (buffer[0] == 1 ? true : false);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "QFTClient encountered an error checking for file existance: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Sends a request for the remote server to shutdown communication with this client.
        /// This should be called as part of a clean shutdown.
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> requestServerDisconnect()
        {
            string command = "?CREF\r";
            byte[] buffer = new byte[1];
            try
            {
                TraceQueue.Trace(this, TracingLevel.Information, "Requesting clean disconnect from {0}", ServerIP);
                if (await ReceiveBytes(command, ref buffer, false) <= 0)
                    return false;

                return (buffer[0] == 1 ? true : false);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "QFTClient encountered an error requesting a remote server disconnect: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Checks remote server for existance of a specified file
        /// </summary>
        /// <param name="remoteDirectory">Directory to check for</param>
        /// <returns>True if file exists, false if not</returns>
        public async Task<bool> DirectoryExists(string remoteDirectory)
        {
            string command = string.Format("?CREB{0}\r", remoteDirectory);
            const int size = 1;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Checking if directory '{0}' exists on {1}", remoteDirectory, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return false;
                }
                return (buffer[0] == 1 ? true : false);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "QFTClient encountered an error checking for directory existance: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Receives a remote file to a local stream instance.
        /// </summary>
        /// <param name="remoteFile">Remote file name to receive.</param>
        /// <param name="destination">Destination stream to write remote file to.</param>
        /// <returns></returns>
        public async Task<bool> ReceiveFile(string remoteFile, Stream destination, FileProgressDelegate progress)
        {
            string command = "?CRE2" + remoteFile + '\r';
            byte[] buffer = new byte[StreamBlockSize];
            int read = 0;
            long totalLength, remainingLength;
            string name;

            if (!IsRunning)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Cannot process QFT Command ReceiveFile, because QFT Client instance is not currently running.");
                return false;
            }
            if (destination == null || !destination.CanWrite)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Cannot process QFT Command ReceiveFile, because source supplied is either null or not readable.");
                return false;
            }
            if (!await FileExists(remoteFile))
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Cannot process QFT Command ReceiveFile, because remote file does not exist.");
                return false;
            }
            totalLength = remainingLength = await GetFileSize(remoteFile);
            if (totalLength == 0)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Cannot process QFT Command ReceiveFile, because remote file size was reported to be 0.");
                return false;
            }

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Attempting to get file '{0}' from {1}", remoteFile, ServerIP);
                    name = Path.GetFileName(remoteFile);
                    destination.Seek(0, SeekOrigin.Begin);
                    do
                    {
                        //Only sends command if read is 0, which can only be the case the first time through this loop
                        bool firstIteration = (read == 0);
                        if (remainingLength > buffer.Length)
                        {
                            read = await ReceiveBytes((firstIteration ? command : string.Empty), ref buffer, firstIteration);
                            destination.Write(buffer, 0, read);
                            remainingLength -= read;
                        }
                        else
                        {
                            //Last packet expected for this file
                            buffer = new byte[remainingLength];
                            read = await ReceiveBytes((read == 0 ? command : string.Empty), ref buffer, false);
                            destination.Write(buffer, 0, read);
                            break;
                        }

                        if (progress != null)
                            progress(totalLength - remainingLength, totalLength, "Getting " + name);

                    } while (read > 0 && remainingLength > 0);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log(ex, "ReceiveFile");
                destination.Flush();
                return false;
            }
        }

        /// <summary>
        /// Gets length of a remote file in bytes.
        /// </summary>
        /// <param name="remoteFile">Server root relative path of file to get size of.</param>
        /// <returns>long value of file size, or 0 if file does not exist.</returns>
        public async Task<long> GetFileSize(string remoteFile)
        {
            string command = string.Format("?CRE0{0}\r", remoteFile);
            const int size = 8;
            byte[] buffer = new byte[size];

            try
            {
                using (await commandLock.LockAsync())
                {
                    TraceQueue.Trace(this, TracingLevel.Information, "Getting file length for '{0}' on {1}", remoteFile, ServerIP);
                    if (await ReceiveBytes(command, ref buffer, true) <= 0)
                        return 0;
                }

                long response = 0;
                for (int i = 0; i < 8; i++)
                    response |= (((long)buffer[i]) << (i * 8));

                return response;
            }
            catch (Exception ex)
            {
                Log(ex, "GetFileSize");
                return 0;
            }
        }

        #endregion

        private void Log(Exception ex, string methodName)
        {
            TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred in {1}: {2}", ex.GetType().Name, methodName, ex.Message);
        }

        private async Task PingWorker_DoWork(object state)
        {
            if (lastCommandTime.AddSeconds(InactivityDisconnectSeconds) >= DateTime.Now)
                return;

            try
            {
                using (await commandLock.LockAsync())
                {
                    if (IsConnected && lastCommandTime.AddSeconds(InactivityDisconnectSeconds) < DateTime.Now)
                    {
                        //We've been inactive for longer than our inactivity timeout.  Shutdown the socket connection now
                        TraceQueue.Trace(this, TracingLevel.Warning, "Closing QFT socket connection to {0} due to {1} seconds of inactivity", ServerIP, InactivityDisconnectSeconds);
                        await requestServerDisconnect();
                        await socket.ShutdownAsync();
                        socket = null;
                    }
                }
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while processing ping worker: {1}", ex.GetType().Name, ex.Message);
            }
        }

    }
}
