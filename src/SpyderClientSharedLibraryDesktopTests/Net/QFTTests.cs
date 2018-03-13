using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using VistaQFT = Vista.QFT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    [TestClass]
    public class QFTTests
    {
        private static QFTClient client;
        private static VistaQFT.QFTServer server;
        protected static string watcherDirectory = @"temp\watcher";	//root relative watcher directory

        public QFTTests()
        {
        }

        [ClassInitialize()]
        public static void StartupQFT(TestContext context)
        {
            server = new VistaQFT.QFTServer(@"c:\", IPAddress.Loopback, QFTClient.SERVER_PORT, watcherDirectory);
            server.Startup();

            client = new QFTClient("127.0.0.1", 7280);
            Assert.IsTrue(client.StartupAsync().Result, "Failed to startup client connection");
        }

        [ClassCleanup()]
        public static void Shutdown()
        {
            if (client != null)
            {
                client.ShutdownAsync().Wait();
                client = null;
            }
            if (server != null)
            {
                server.Shutdown();
                server = null;
            }
        }

        [TestInitialize()]
        public void SetUp()
        {
            var tcs = new TaskCompletionSource<bool>();

            Task.Run(async () =>
                {
                    try
                    {
                        if (!await client.Ping())
                        {
                            Assert.IsTrue(await client.StartupAsync(), "Failed to start client for test");
                        }
                        tcs.TrySetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                });

            tcs.Task.Wait();
        }

        protected async Task sendFile(long size)
        {
            string clientFile = createDummyFile(Path.GetTempFileName(), size);
            string serverFile = Path.GetTempFileName();
            try
            {
                using (Stream stream = File.OpenRead(clientFile))
                {
                    Assert.IsTrue(await client.SendFile(stream, client.ConvertAbsolutePathToRelative(serverFile), null), "SendFile returned false -- send failed");
                }
                Assert.IsTrue(File.Exists(serverFile), "SendFile returned true, but file does not exist.");
                compareFiles(clientFile, serverFile);
            }
            finally
            {
                TryDeleteFile(serverFile);
                TryDeleteFile(clientFile);
            }
        }

        [TestMethod()]
        public async Task SendSmallFile()
        {
            await sendFile(65535);
        }

        [TestMethod()]
        public async Task SendLargeFile()
        {
            await sendFile(331667986);
        }

        public async Task receiveFile(long size)
        {
            string serverFile = createDummyFile(Path.GetTempFileName(), size);
            string clientFile = Path.GetTempFileName();
            try
            {
                TryDeleteFile(clientFile);

                using (Stream clientFileStream = File.Create(clientFile))
                {
                    Assert.IsTrue(await client.ReceiveFile(client.ConvertAbsolutePathToRelative(serverFile), clientFileStream, null), "ReceiveFile returned false -- receive failed");
                }

                Assert.IsTrue(File.Exists(serverFile), "ReceivedFile returned true, but file does not exist.");
                compareFiles(serverFile, clientFile);
            }
            finally
            {
                TryDeleteFile(serverFile);
                TryDeleteFile(clientFile);
            }
        }

        [TestMethod()]
        public async Task ReceiveSmallFile()
        {
            await receiveFile(65535);
        }

        [TestMethod()]
        public async Task ReceiveLargeFile()
        {
            //Receive 500 meg file
            await receiveFile(331667986);
        }


        [TestMethod()]
        public async Task CreateDirectory()
        {
            string tempDir = @"c:\temp\nunittestfolder";
            try
            {
                TryDeleteFile(tempDir);

                //Make sure the directory doesn't exist
                Assert.IsFalse(Directory.Exists(tempDir));
                Assert.IsTrue(await client.CreateDirectory(client.ConvertAbsolutePathToRelative(tempDir)), "Failed to create specified directory");
                Assert.IsTrue(Directory.Exists(tempDir), "QFT Client returned true from CreateDirectory, but directory does not exist");
            }
            finally
            {
                TryDeleteDirectory(tempDir);
            }
        }

        [TestMethod()]
        public async Task DirectoryExists()
        {
            string tempDir = @"c:\temp\nunittestfolder";
            try
            {
                //Test directory not exists first
                TryDeleteDirectory(tempDir);

                Assert.IsFalse(await client.DirectoryExists(client.ConvertAbsolutePathToRelative(tempDir)), "Client reported directory existed when it did not.");

                //Test directory does exist
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                Assert.IsTrue(await client.DirectoryExists(client.ConvertAbsolutePathToRelative(tempDir)), "Client reported directory did not existe when it did.");
            }
            finally
            {
                TryDeleteDirectory(tempDir);
            }
        }

        [TestMethod()]
        public async Task DeleteDirectory()
        {
            string tempDir = @"c:\temp\nunittestfolder";
            try
            {
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                //Put a file in the directory to make sure it removes it at well
                createDummyFile(Path.Combine(tempDir, "dummyfile.txt"), 1024);

                //Make sure the directory exists
                Assert.IsTrue(Directory.Exists(tempDir));
                Assert.IsTrue(await client.DeleteDirectory(client.ConvertAbsolutePathToRelative(tempDir)), "Failed to create specified directory");
                Assert.IsFalse(Directory.Exists(tempDir), "QFT Client returned true from DeleteDirectory, but directory exists");
            }
            finally
            {
                TryDeleteDirectory(tempDir);
            }
        }

        [TestMethod()]
        public async Task GetDirectories()
        {
            string tempDir = @"c:\temp\nunittestfolder";
            try
            {
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                for (int i = 0; i < 100; i++)
                    Directory.CreateDirectory(Path.Combine(tempDir, "Directory " + i.ToString()));

                string[] expected = Directory.GetDirectories(tempDir);
                for (int i = 0; i < expected.Length; i++)
                    expected[i] = new DirectoryInfo((string)expected[i]).Name;

                string[] actual = await client.GetDirectories(client.ConvertAbsolutePathToRelative(tempDir));

                Assert.AreEqual(expected.Length, actual.Length, "Directory count was unexpected");
                for (int i = 0; i < actual.Length; i++)
                    Assert.AreEqual(expected[i], actual[i], "Directory at index {0} did not match", i);
            }
            finally
            {
                TryDeleteDirectory(tempDir);
            }
        }

        [TestMethod()]
        public async Task GetFiles()
        {
            string tempDir = @"c:\temp\nunittestfolder";
            try
            {
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                for (int i = 0; i < 100; i++)
                    createDummyFile(Path.Combine(tempDir, "dummyFile." + i.ToString()), 64);

                string[] expected = Directory.GetFiles(tempDir);
                for (int i = 0; i < expected.Length; i++)
                    expected[i] = Path.GetFileName((string)expected[i]);

                string[] actual = await client.GetFiles(client.ConvertAbsolutePathToRelative(tempDir));

                Assert.AreEqual(expected.Length, actual.Length, "File count was unexpected");
                for (int i = 0; i < actual.Length; i++)
                    Assert.AreEqual(expected[i], actual[i], "File at index {0} did not match", i);
            }
            finally
            {
                TryDeleteDirectory(tempDir);
            }
        }

        [TestMethod()]
        public async Task GetFileSize()
        {
            string tempFile = createDummyFile();
            try
            {
                long expected = new FileInfo(tempFile).Length;
                long actual = await client.GetFileSize(client.ConvertAbsolutePathToRelative(tempFile));
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                TryDeleteFile(tempFile);
            }
        }

        [TestMethod()]
        public async Task FileExists()
        {
            string dummyFile = Path.GetTempFileName();
            try
            {
                TryDeleteFile(dummyFile);

                //Make sure file doesn't exist
                Assert.IsFalse(await client.FileExists(client.ConvertAbsolutePathToRelative(dummyFile)), "Client reported file existed when it did not.");

                //Make sure it correctly identifies the file if it exists
                createDummyFile(dummyFile, 1024);
                Assert.IsTrue(await client.FileExists(client.ConvertAbsolutePathToRelative(dummyFile)), "Client reported file did not exist when it did.");
            }
            finally
            {
                TryDeleteFile(dummyFile);
            }
        }

        [TestMethod()]
        public async Task DeleteFile()
        {
            string dummyFile = createDummyFile();
            try
            {
                Assert.IsTrue(File.Exists(dummyFile), "Dummy file does not exist to be deleted.");
                Assert.IsTrue(await client.DeleteFile(client.ConvertAbsolutePathToRelative(dummyFile)), "DeleteFile method failed");
                Assert.IsFalse(File.Exists(dummyFile), "Dummy file was not successfully deleted.");
            }
            finally
            {
                TryDeleteFile(dummyFile);
            }
        }

        [TestMethod()]
        public async Task Ping()
        {
            Assert.IsTrue(await client.Ping(), "Ping failed");
        }

        [TestMethod()]
        public async Task GetRemoteTimeOffset()
        {
            const int maxSkewInMs = 250;
            TimeSpan offset = await client.GetRemoteTimeOffset();
            Assert.IsTrue(Math.Abs(offset.TotalMilliseconds) < maxSkewInMs, "Time offset was too large.  Offset was {0}", offset);
        }

        [TestMethod()]
        public async Task GetCreationTime()
        {
            string dummy = createDummyFile();
            try
            {
                DateTime expected = File.GetCreationTime(dummy);
                DateTime actual = await client.GetCreationTime(client.ConvertAbsolutePathToRelative(dummy));
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                TryDeleteFile(dummy);
            }
        }

        [TestMethod()]
        public async Task SetModifiedFileTime()
        {
            string dummy = createDummyFile();
            try
            {
                DateTime setTime = DateTime.Now.Subtract(new TimeSpan(2, 3, 4, 5, 6));
                Assert.IsTrue(await client.SetModifiedTime(client.ConvertAbsolutePathToRelative(dummy), setTime), "Failed to set modified time");

                //Make sure the retuned time is the same as the local file itself
                DateTime expected = File.GetLastWriteTime(dummy);
                Assert.AreEqual(expected, setTime);
            }
            finally
            {
                TryDeleteFile(dummy);
            }
        }

        [TestMethod()]
        public async Task GetModifiedTime()
        {
            string dummy = createDummyFile();
            try
            {
                DateTime expected = File.GetLastWriteTime(dummy);
                DateTime actual = await client.GetModifiedTime(client.ConvertAbsolutePathToRelative(dummy));
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                TryDeleteFile(dummy);
            }
        }

        [TestMethod()]
        public void FolderCreateCompareTest()
        {
            string tempDir = @"c:\temp\nunittestfolder";
            string tempDir2 = @"c:\temp\nunittestfolder2";
            try
            {
                this.createRecursiveFolder(tempDir);
                this.createRecursiveFolder(tempDir2);
                Assert.IsTrue(compareTwoFolders(tempDir, tempDir2));
            }
            finally
            {
                TryDeleteDirectory(tempDir);
                TryDeleteDirectory(tempDir2);
            }
        }

        [TestMethod()]
        public async Task ReceiveFileFiftyTimes()
        {
            string serverFile = createDummyFile(Path.GetTempFileName(), 65536);
            string clientFile = Path.GetTempFileName();
            try
            {
                for (int i = 0; i < 50; i++)
                {
                    TryDeleteFile(clientFile);
                    using (var stream = File.Create(clientFile))
                    {
                        Assert.IsTrue(await client.ReceiveFile(client.ConvertAbsolutePathToRelative(serverFile), stream, null), "ReceiveFile returned false -- receive failed");
                    }
                    Assert.IsTrue(File.Exists(serverFile), "ReceivedFile returned true, but file does not exist.");
                    compareFiles(serverFile, clientFile);
                }
            }
            finally
            {
                TryDeleteFile(serverFile);
                TryDeleteFile(clientFile);
            }
        }

        [TestMethod()]
        public async Task SendAndOverwriteOneHundredTimes()
        {
            char fillChar = 'a';
            string serverFile = Path.GetTempFileName();
            string clientFile = Path.GetTempFileName();

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    createDummyFile(clientFile, 65535, fillChar++);

                    using (Stream stream = File.OpenRead(clientFile))
                    {
                        Assert.IsTrue(await client.SendFile(stream, client.ConvertAbsolutePathToRelative(serverFile), null));
                    }
                    compareFiles(clientFile, serverFile);
                }
            }
            finally
            {
                TryDeleteFile(serverFile);
                TryDeleteFile(clientFile);
            }
        }

        [TestMethod()]
        public async Task FileSendTime()
        {
            const int megabytes = 10;
            long fileSize = megabytes * 1024 * 1024;
            string tempFile = Path.GetTempFileName();
            createDummyFile(tempFile, fileSize);
            string destFile = Path.GetTempFileName();

            try
            {
                DateTime startTime = DateTime.Now;
                using (Stream stream = File.OpenRead(tempFile))
                {
                    await client.SendFile(stream, client.ConvertAbsolutePathToRelative(destFile), null);
                }
                DateTime endTime = DateTime.Now;
                TimeSpan span = endTime.Subtract(startTime);
                Console.WriteLine("SendingB {0}MB file completed in {1} ({2}k/s", megabytes, span, ((float)fileSize / 1024f) / (float)span.TotalSeconds);

                //Verify the file copied properly
                compareFiles(tempFile, destFile);

                //Todo: Enforce time constraints and fail if outside constraint
            }
            finally
            {
                TryDeleteFile(tempFile);
                TryDeleteFile(destFile);
            }
        }

        [TestMethod()]
        public async Task FileReceiveTime()
        {
            const int megabytes = 10;
            long fileSize = megabytes * 1024 * 1024;
            string tempFile = Path.GetTempFileName();
            createDummyFile(tempFile, fileSize);
            string destFile = Path.GetTempFileName();

            try
            {
                DateTime startTime = DateTime.Now;
                using (Stream stream = File.Create(destFile))
                {
                    await client.ReceiveFile(client.ConvertAbsolutePathToRelative(tempFile), stream, null);
                }
                DateTime endTime = DateTime.Now;
                TimeSpan span = endTime.Subtract(startTime);
                Console.WriteLine("Receiving {0}MB file completed in {1} ({2}k/s", megabytes, span, ((float)fileSize / 1024f) / (float)span.TotalSeconds);

                //Verify files sent were sent properly
                compareFiles(tempFile, destFile);

                //Todo: Enforce time constraints and fail if outside constraint
            }
            finally
            {
                TryDeleteFile(tempFile);
                TryDeleteFile(destFile);
            }
        }

        protected void progressHandler(long bytes, long total, string fileName)
        {
            Console.WriteLine("{0} {1} / {2}", fileName, bytes, total);
        }

        private void TryDeleteFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch { }
        }

        private void TryDeleteDirectory(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);
            }
            catch { }
        }

        
        #region Generic File Generation / Compare methods

        protected void compareFiles(string origonalFile, string compareFile)
        {
            if (!File.Exists(origonalFile))
                throw new FileNotFoundException("Failed to find origonal file to perform compare.");

            if (!File.Exists(compareFile))
                throw new FileNotFoundException("Failed to find compare file to perform compare on.");

            FileStream origonalStream = null;
            FileStream compareStream = null;

            try
            {
                origonalStream = File.OpenRead(origonalFile);
                compareStream = File.OpenRead(compareFile);

                if (origonalStream.Length != compareStream.Length)
                    throw new Exception(string.Format("Origonal file and compare file are different sizes.  Origonal: {0} Compare: {1}", origonalStream.Length, compareStream.Length));

                const int bufferSize = 8096;
                byte[] origBuffer = new byte[bufferSize];
                byte[] compBuffer = new byte[bufferSize];
                long fileSize = origonalStream.Length;
                int index = 0;
                while(index < fileSize)
                {
                    int origRead = origonalStream.Read(origBuffer, 0, bufferSize);
                    int compareRead = compareStream.Read(compBuffer, 0, bufferSize);
                    Assert.AreEqual(origRead, compareRead, "Read sized from streams did not match");

                    for(int i=0 ; i<origRead ; i++)
                    {
                        if(origBuffer[i] != compBuffer[i])
                            Assert.Fail("Buffers did not match at index " + i);
                    }

                    index += origRead;
                }
            }
            finally
            {
                if (origonalStream != null)
                {
                    origonalStream.Close();
                    origonalStream = null;
                }
                if (compareStream != null)
                {
                    compareStream.Close();
                    compareStream = null;
                }
            }
        }

        protected string createDummyFile(string tempFile, long size, char fillChar)
        {
            StreamWriter writer = null;
            try
            {
                TryDeleteFile(tempFile);
                writer = File.CreateText(tempFile);
                for (long i = 0; i < size; i++)
                    writer.Write(fillChar);

                writer.Flush();
                return tempFile;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                    writer = null;
                }
            }
        }

        /// <summary>
        /// Creates a dummy file of a specified byte size.
        /// </summary>
        /// <param name="size">Size in bytes of file to create</param>
        /// <returns></returns>
        protected string createDummyFile(string tempFile, long size)
        {
            return createDummyFile(tempFile, size, 'H');
        }

        protected string createDummyFile()
        {
            return createDummyFile(Path.GetTempFileName(), 1024);
        }

        protected void createRecursiveFolder(string tempDir)
        {
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            for (int i = 0; i < 10; i++)
            {
                string subDir = Path.Combine(tempDir, "Directory " + i.ToString());
                Directory.CreateDirectory(subDir);
                for (int j = 0; j < 10; j++)
                    createDummyFile(Path.Combine(subDir, "File." + j.ToString()), 1024);
            }
        }

        protected bool compareTwoFolders(string origonal, string compare)
        {
            string[] origonalSubs = Directory.GetDirectories(origonal);
            string[] compareSubs = Directory.GetDirectories(compare);
            if (compareSubs.Length != origonalSubs.Length)
                return false;

            //check subdirs
            foreach (string subDir in origonalSubs)
            {
                bool exists = false;
                foreach (string subCompare in compareSubs)
                {
                    string strCompare = new DirectoryInfo(subCompare).Name;
                    string strDir = new DirectoryInfo(subDir).Name;
                    if (strCompare == strDir)
                    {
                        exists = true;

                        //recursively search folders
                        if (!compareTwoFolders(subDir, subCompare))
                            return false;

                        break;
                    }
                }

                if (!exists)
                    return false;
            }

            //check files
            string[] origonalFiles = Directory.GetFiles(origonal);
            string[] compareFiles = Directory.GetFiles(compare);
            if (origonalFiles.Length != compareFiles.Length)
                return false;

            foreach (string subFile in origonalFiles)
            {
                bool exists = false;
                foreach (string subCompare in compareFiles)
                    if (Path.GetFileName(subCompare) == Path.GetFileName(subFile))
                    {
                        exists = true;
                        break;
                    }

                if (!exists)
                    return false;
            }
            return true;
        }

        #endregion
    }
}
