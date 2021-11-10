using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    public abstract class SpyderClientTestBase
    {
        public const string serverIP = "127.0.0.1";

        private static ISpyderClient udp;
        private static TestUdpServer server;
        private readonly Func<HardwareType, string, Task<ISpyderClient>> getClient;

        protected SpyderClientTestBase(Func<HardwareType, string, Task<ISpyderClient>> getClient)
        {
            this.getClient = getClient;
        }

        [TestInitialize]
        public void ClassInitialize()
        {
            //Startup a local test server
            if (serverIP == IPAddress.Loopback.ToString() && server == null)
                server = new TestUdpServer();

            if (udp == null)
            {
                udp = getClient(HardwareType.SpyderX80, serverIP).Result;
                if (!udp.StartupAsync().Result)
                {
                    udp = null;
                    Assert.Inconclusive("Failed to startup UDP client");
                }
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (udp != null)
            {
                udp.ShutdownAsync().Wait();
                udp = null;
            }

            if (server != null)
            {
                server.Dispose();
                server = null;
            }
        }

        #region Data

        [TestMethod]
        public async Task GetSourcesTest()
        {
            await GetDataTest(() => udp.GetSources());
        }

        [TestMethod]
        public async Task GetSourceTest()
        {
            //Task<Source> GetSource(int sourceRegisterID);
            //Task<Source> GetSource(IRegister sourceRegister);
            //Task<Source> GetSource(string sourceName);

            var sources = await GetDataTest(() => udp.GetSources());
            foreach (var expected in sources)
            {
                //Get by name
                Source actual = await udp.GetSource(expected.Name);
                Assert.AreEqual(expected, actual, "Failed to get matching source by name");

                //Get by register ID
                actual = await udp.GetSource(expected.RegisterID);
                Assert.AreEqual(expected, actual, "Failed to get matching source by Register ID");

                //Get by register
                actual = await udp.GetSource(expected);
                Assert.AreEqual(expected, actual, "Failed to get matching source by Register");
            }
        }

        [TestMethod]
        public async Task GetFunctionKeysTest()
        {
            await GetDataTest(() => udp.GetFunctionKeys());
        }

        [TestMethod]
        public async Task GetCommandKeysTest()
        {
            await GetDataTest(() => udp.GetCommandKeys());
        }

        [TestMethod]
        public async Task GetStillsTest()
        {
            await GetDataTest(() => udp.GetStills());
        }

        [TestMethod]
        public async Task GetPlayItemsTest()
        {
            await GetDataTest(() => udp.GetPlayItems());
        }

        [TestMethod]
        public async Task GetTreatmentsTest()
        {
            await GetDataTest(() => udp.GetTreatments());
        }

        private async Task<List<T>> GetDataTest<T>(Func<Task<List<T>>> getList)
        {
            var results = await getList();
            Assert.IsNotNull(results, "Failed to query");
            Assert.AreNotEqual(0, results.Count(), "No items returned");
            return results;
        }

        #endregion

        #region Output Configuration

        [TestMethod]
        public async Task FreezeAndUnFreezeOutputTest()
        {
            Assert.IsTrue(await udp.FreezeOutput(0), "Failed to freeze output");
            Assert.IsTrue(await udp.UnFreezeOutput(0), "Failed to freeze output");
        }

        #endregion

        #region Layer Control

        [TestMethod]
        public async Task FreezeAndUnFreezeLayerTest()
        {
            Assert.IsTrue(await udp.FreezeLayer(2), "Failed to freeze layer");
            Assert.IsTrue(server.Sys.GetLayer(2).IsFrozen, "Layer did not actually freeze");

            Assert.IsTrue(await udp.UnFreezeLayer(2), "Failed to un-freeze layer");
            Assert.IsFalse(server.Sys.GetLayer(2).IsFrozen, "Layer did not actually un-freeze");
        }

        #endregion

        #region PixelSpace Interaction

        [TestMethod]
        public async Task GetPixelSpacesTest()
        {
            var pixelSpaces = await GetDataTest(() => udp.GetPixelSpaces());
            Assert.IsNotNull(pixelSpaces, "Failed to get pixelspaces");
            Assert.AreNotEqual(0, pixelSpaces.Count, "No PixelSpaces were returned");
        }

        public async Task GetPixelSpaceTest()
        {
            var pixelSpaces = await udp.GetPixelSpace((int)server.Sys.PixelSpaces.GetKey(0));
            Assert.IsNotNull(pixelSpaces, "Failed to get PixelSpace");
        }

        #endregion

        [TestMethod]
        public async Task GetDataIOProcessorStatusTest()
        {
            var drawingData = server.Sys.GetDrawingData();
            var status = await udp.GetDataIOProcessorStatus();
            Assert.IsNotNull(status, "Failed to get status");
            Assert.AreEqual(drawingData.ProgressString, status.Message, "Incorrect progress string");
            Assert.AreEqual(drawingData.PercentComplete, status.PercentCompleteRaw, "Percent Complete incorrect");

            if(drawingData.PercentComplete == 0 || drawingData.PercentComplete == 101)
            {
                Assert.IsTrue(status.IsIdle, "Expected processor to be idle");
                Assert.AreEqual(drawingData.PercentComplete, status.PercentComplete, "Expected PercentComplete to match DrawingData");
            }
            else
            {
                Assert.IsFalse(status.IsIdle, "Expected processor to not be idle");
                Assert.AreEqual(drawingData.PercentComplete, status.PercentComplete, "Incorrect PercentComplete");
            }
        }

        [TestMethod]
        public async Task WaitForDataIOProcessorToBeIdleTest()
        {
            Assert.IsTrue(await udp.WaitForDataIOProcessorToBeIdle(TimeSpan.FromSeconds(30)), "Failed to wait for idle");
        }
    }
}
