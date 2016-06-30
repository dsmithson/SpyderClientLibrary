using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder.Client.Images
{
    [TestClass]
    public class ThumbnailManagerBaseTests
    {
        [TestMethod]
        public async Task GetThumbnailTest()
        {
            ImageSize? sizeRequested = null;
            int? identifierRequested = null;
            var reset = new ManualResetEvent(false);
            var manager = new MockThumbnailManager();
            manager.CreateImageRequested += (e) =>
                {
                    sizeRequested = e.Size;
                    identifierRequested = e.Identifier;
                    e.Result = e.Size.ToString();
                    reset.Set();
                };
            Assert.IsTrue(await manager.StartupAsync(), "Failed to startup Thumbnail Manager");

            //Get the initial thumbnail reference, twice.  We'll ensure they are the same object reference, and then ensure that the refresh methods haven't actually been called
            var image = manager.GetThumbnail(0);
            Assert.AreSame(image, manager.GetThumbnail(0), "Expected same object reference to be returned when requesting the same thumbnail record twice");
            Assert.IsNull(sizeRequested, "A thumbnail size has been requested for generation before the resource was requested");

            //Now lets request an image from our thumbnail instance, and we should get an event fired up through our manager
            //NOTE:  Using an ManualResetEvent to manage synchronization here
            string initialValue = image.SmallImage;
            Assert.AreEqual(MockThumbnailImage.DefaultImageString, initialValue, "Did not receive the expected initial value upon first request of the resource");
            Assert.IsTrue(reset.WaitOne(1000), "Manager failed to process image update request");
            Assert.AreEqual(ImageSize.Small, sizeRequested.Value, "Incorrect image size requested");
            Assert.AreEqual(0, identifierRequested.Value, "Incorrect image identifier requested");
        }
    }
}
