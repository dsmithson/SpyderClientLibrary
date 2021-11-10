using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Spyder.Client.Net.DrawingData.Deserializers
{
    [TestClass]
    public class DrawingDataDeserializerTests
    {
        [TestMethod]
        public async Task Deserialize_V2_10_8_Test()
        {
            await RunTest(
                new DrawingDataDeserializer_Version8(),
                "Spyder.Client.Resources.TestDrawingData.DrawingData_2.10.8_V8.bin");
        }

        [TestMethod]
        public async Task Deserialize_V3_5_9_Test()
        {
            await RunTest(
                new DrawingDataDeserializer_Version19(),
                "Spyder.Client.Resources.TestDrawingData.DrawingData_3.5.9_V19.bin");
        }

        [TestMethod]
        public async Task Deserialize_V4_0_1_Test()
        {
            await RunTest(
                new DrawingDataDeserializer_Version19(),
                "Spyder.Client.Resources.TestDrawingData.DrawingData_4.0.1_V19.bin");
        }

        [TestMethod]
        public async Task Deserialize_V4_1_0_Test()
        {
            await RunTest(
                new DrawingDataDeserializer_Version20("4.1.0"),
                "Spyder.Client.Resources.TestDrawingData.DrawingData_4.1.0_V20.bin");
        }

        [TestMethod]
        public async Task Deserialize_V5_4_1_Test()
        {
            await RunTest(
                new DrawingDataDeserializer_Version54("5.4.1"),
                "Spyder.Client.Resources.TestDrawingData.DrawingData_5.4.1_V54.bin");
        }

        private async Task RunTest(IDrawingDataDeserializer deserializer, string embeddedResourceName)
        {
            var assy = Assembly.GetExecutingAssembly();
            var stream = assy.GetManifestResourceStream(embeddedResourceName);
            if (stream == null)
                Assert.Fail("Failed to read test data stream: " + embeddedResourceName);

            byte[] buffer = new byte[stream.Length];
            int readCount = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (readCount != buffer.Length)
                Assert.Inconclusive("Failed to load test data");

            DrawingData drawingData = deserializer.Deserialize(buffer);
            Assert.IsNotNull(drawingData, "Failed to deserialize");
        }
    }
}
