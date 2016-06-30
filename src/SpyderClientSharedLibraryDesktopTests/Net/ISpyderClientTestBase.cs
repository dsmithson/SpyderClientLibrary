using Knightware.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    public abstract class ISpyderClientTestBase
    {
        public const string serverIP = "192.168.1.175";

        private static ISpyderClient udp;
        private readonly Func<string, Task<ISpyderClient>> getClient;
        
        protected ISpyderClientTestBase(Func<string, Task<ISpyderClient>> getClient)
        {
            this.getClient = getClient;
        }

        [TestInitialize]
        public void ClassInitialize()
        {
            if (udp == null)
            {
                udp = getClient(serverIP).Result;
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
        }
        
        [TestMethod]
        public async Task GetPixelSpacesTest()
        {
            await GetDataTest(() => udp.GetPixelSpaces());
        }

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
    }
}
