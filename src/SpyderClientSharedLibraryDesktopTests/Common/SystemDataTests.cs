using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Spyder.Client.Common
{
    [TestClass]
    public class SystemDataTests : SystemData
    {
        private XDocument GetTestSystemConfigFile()
        {
            return XDocument.Load(GetTestSystemConfigStream());
        }

        private XDocument GetTestScriptsFile()
        {
            return XDocument.Load(GetTestScriptsStream());
        }

        private Stream GetTestSystemConfigStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.SystemConfiguration.xml");
        }

        private Stream GetTestScriptsStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.Scripts.xml");
        }

        [TestMethod]
        public void LoadDataTest()
        {
            Assert.IsTrue(LoadData(GetTestSystemConfigStream(), GetTestScriptsStream()), "Failed to load data");
        }

        [TestMethod]
        public async Task LoadDataAsyncTest()
        {
            Assert.IsTrue(await LoadDataAsync(GetTestSystemConfigStream(), GetTestScriptsStream()), "Failed to load data");
        }

        [TestMethod]
        public void LoadScriptsTest()
        {
            var scripts = ParseScripts(GetTestScriptsFile());
            Assert.IsNotNull(scripts, "Failed to parse scripts");
            Assert.AreNotEqual(0, scripts.Count, "Failed to parse any scripts");
        }

        [TestMethod]
        public void LoadInputConfigsTest()
        {
            var InputConfigs = ParseInputConfigs(GetTestSystemConfigFile());
            Assert.IsNotNull(InputConfigs, "Failed to parse InputConfigs");
            Assert.AreNotEqual(0, InputConfigs.Count, "Failed to parse any InputConfigs");
        }

        [TestMethod]
        public void LoadPixelSpacesTest()
        {
            var pixelSpaces = ParsePixelSpaces(GetTestSystemConfigFile());
            Assert.IsNotNull(pixelSpaces, "Failed to parse pixelSpaces");
            Assert.AreNotEqual(0, pixelSpaces.Count, "Failed to parse any PixelSpaces");
        }

        [TestMethod]
        public void LoadSourcesTest()
        {
            var sources = ParseSources(GetTestSystemConfigFile());
            Assert.IsNotNull(sources, "Failed to parse Sources");
            Assert.AreNotEqual(0, sources.Count, "Failed to parse any Sources");
        }

        [TestMethod]
        public void LoadPlayItemsTest()
        {
            var PlayItems = ParsePlayItems(GetTestSystemConfigFile());
            Assert.IsNotNull(PlayItems, "Failed to parse PlayItems");
            Assert.AreNotEqual(0, PlayItems.Count, "Failed to parse any PlayItems");
        }

        [TestMethod]
        public void LoadTreatmentsTest()
        {
            var treatments = ParseTreatments(GetTestSystemConfigFile());
            Assert.IsNotNull(treatments, "Failed to parse treatments");
            Assert.AreNotEqual(0, treatments.Count, "Failed to parse any treatments");
        }

        [TestMethod]
        public void LoadRoutersTest()
        {
            var routers = ParseRouters(GetTestSystemConfigFile());
            Assert.IsNotNull(routers, "Failed to parse routers");
            Assert.AreNotEqual(0, routers.Count, "Failed to parse any routers");
        }

        [TestMethod]
        public void LoadRegisterListTest()
        {
            foreach (RegisterType type in Enum.GetValues(typeof(RegisterType)))
            {
                var registers = ParseRegisterList(GetTestSystemConfigFile(), type);
                Assert.IsNotNull(registers, "Failed to parse {0} register list", type);
                Assert.AreNotEqual(0, registers.Count, "No registers were parsed from {0} list", type);
            }
        }
    }
}
