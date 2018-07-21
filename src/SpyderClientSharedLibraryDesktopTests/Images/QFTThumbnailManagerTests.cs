using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder.Client.Images
{
    [TestClass]
    public class QFTThumbnailManagerTests
    {
        private const string fileProcessedResult = "File Processed";

        private static string remoteImagePath;
        private static string localImagePath;
        private static Vista.QFT.QFTServer qftServer;
        private static Spyder.Client.Net.QFTClient qftClient;
        
        private Dictionary<string, ManualResetEvent> imageProcessedEvents;
        private MockQFTThumbnailManager thumbnailManager;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            //Create test directories
            remoteImagePath = Path.Combine(Path.GetTempPath(), "QFTThumbnailTests", "server");
            localImagePath = Path.Combine(Path.GetTempPath(), "QFTThumbnailTests", "client");
            CleanDirectories(true, remoteImagePath, localImagePath);

            //Create server images
            for (int i = 0; i < 5; i++)
            {
                string imageFile= Path.Combine(remoteImagePath, string.Format("Still Image {0}.bmp", (i+1)));
                UnitTestHelper.CreateStillImage(imageFile);
            }

            //Initialize our local folder object that will be used for our client directory
            if (!Directory.Exists(localImagePath))
                Directory.CreateDirectory(localImagePath);

            //Initialize our QFT Server
            qftServer = new Vista.QFT.QFTServer();
            qftServer.Startup();

            //Initialize our QFT Client
            qftClient = new Spyder.Client.Net.QFTClient("127.0.0.1");
            Assert.IsTrue(qftClient.StartupAsync().Result, "Failed to initialize QFT Client");
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            if (qftClient != null)
            {
                qftClient.ShutdownAsync().Wait();
                qftClient = null;
            }

            if (qftServer != null)
            {
                qftServer.Shutdown();
                qftServer = null;
            }

            CleanDirectories(false, localImagePath, remoteImagePath);
            localImagePath = null;
            remoteImagePath = null;
        }

        [TestInitialize]
        public void TestSetup()
        {
            //Reset image process event awaiters
            imageProcessedEvents = new Dictionary<string, ManualResetEvent>();

            thumbnailManager = new MockQFTThumbnailManager(localImagePath, (serverIP) => Task.FromResult(@"c:\spyder\images"));
            thumbnailManager.ProcessImageStreamRequested += thumbnailManager_ProcessImageStreamRequested;
            Assert.IsTrue(thumbnailManager.StartupAsync().Result, "Failed to initialize thumbnail manager");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (thumbnailManager != null)
            {
                thumbnailManager.ProcessImageStreamRequested -= thumbnailManager_ProcessImageStreamRequested;
                thumbnailManager.ShutdownAsync().Wait();
                thumbnailManager = null;
            }

            imageProcessedEvents = null;

            //Remove any previously downloaded files from the local client directory
            CleanDirectories(true, localImagePath);
        }

        void thumbnailManager_ProcessImageStreamRequested(object sender, ProcessImageStreamEventArgs<QFTThumbnailIdentifier, string> e)
        {
            e.Result = fileProcessedResult;

            //Set the manual reset event if registered to let tests know that the file has been 'processed'
            string file = e.Identifier.FileName.ToLower();
            if (imageProcessedEvents.ContainsKey(file))
                imageProcessedEvents[file].Set();
        }

        [TestMethod]
        public async Task GetImageTest()
        {
            string fileName = Path.GetFileName(Directory.GetFiles(remoteImagePath).First());
            await GetImagesTest(fileName);
        }

        [TestMethod]
        public async Task GetImagesTest()
        {
            string[] fileNames = Directory.GetFiles(remoteImagePath).Select(f => Path.GetFileName(f)).ToArray();
            await GetImagesTest(fileNames);
        }

        private async Task GetImagesTest(params string[] fileNames)
        {
            List<QFTThumbnailIdentifier> identifiers = fileNames.Select(f => new QFTThumbnailIdentifier("127.0.0.1", f)).ToList();
            ManualResetEvent[] resetEvents = fileNames.Select(f => GetResetEventForFile(f)).ToArray();
            var thumbnails = identifiers.Select(f => thumbnailManager.GetThumbnail(f)).ToList();

            //Pull the small image to start the rendering process
            foreach (var thumbnail in thumbnails)
            {
                string s = thumbnail.SmallImage;
            }

            //Wait to be signalled that the image has been downloaded and processing has happened
            //Note:  WaitHandle.WaitAll threw a notsupported exception...
            foreach (ManualResetEvent resetEvent in resetEvents)
            {
                Assert.IsTrue(resetEvent.WaitOne(10000), "Failed to be signalled for one or more files");
            }

            //Ensure our file was created in the cache folder
            foreach (string fileName in fileNames)
            {
                string localFile = Path.Combine(localImagePath, "127.0.0.1", "Images", fileName);
                string remoteFile = Path.Combine(remoteImagePath, fileName);
                Assert.IsTrue(File.Exists(localFile), "Failed to create local file: " + fileName);
                UnitTestHelper.CompareFilesAssert(remoteFile, localFile);
            }

            //Wait a second for the results to be set
            await Task.Delay(1000);
            foreach (var thumbnail in thumbnails)
            {
                Assert.AreEqual(fileProcessedResult, thumbnail.SmallImage, "Resulting image failed to be set on our thumbnail");
            }
        }

        private static void CleanDirectories(bool ensureCreated, params string[] directories)
        {
            foreach (string directory in directories)
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);

                if(ensureCreated)
                    Directory.CreateDirectory(directory);
            }
        }

        private ManualResetEvent GetResetEventForFile(string fileName)
        {
            string lower = fileName.ToLower();
            if (!imageProcessedEvents.ContainsKey(lower))
                imageProcessedEvents.Add(lower, new ManualResetEvent(false));

            return imageProcessedEvents[lower];
        }
    }
}
