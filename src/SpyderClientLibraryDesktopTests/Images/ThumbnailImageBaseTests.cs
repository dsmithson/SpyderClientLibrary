using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Spyder.Client.Images
{
    [TestClass]
    public class ThumbnailImageBaseTests
    {
        [TestMethod]
        public void GetSmallImageTest()
        {
            GetImageTest(ImageSize.Small, (image) => image.SmallImage, "SmallImage");
        }

        [TestMethod]
        public void GetMediumImageTest()
        {
            GetImageTest(ImageSize.Medium, (image) => image.MediumImage, "MediumImage");
        }

        [TestMethod]
        public void GetLargeImageTest()
        {
            GetImageTest(ImageSize.Large, (image) => image.LargeImage, "LargeImage");
        }

        private void GetImageTest(ImageSize size, Func<MockThumbnailImage, string> getPropertyValue, string propertyName)
        {
            ImageSize? imageRequested = null;
            bool thumbnailChangeNotified = false;
            bool isLoadingNotifiedTrue = false;
            bool isLoadingNotifiedFalse = false;

            var image = new MockThumbnailImage();
            image.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == propertyName)
                    {
                        thumbnailChangeNotified = true;
                    }
                    else if (e.PropertyName == "IsLoading" + propertyName)
                    {
                        //Check to see if the value was set to true
                        PropertyInfo prop = image.GetType().GetProperty("IsLoading" + propertyName);
                        bool isLoading = (bool)prop.GetValue(image);
                        if (isLoading)
                        {
                            if (isLoadingNotifiedTrue)
                                Assert.Fail("IsLoading notified more than one more time with a 'true' value");

                            isLoadingNotifiedTrue = true;
                        }
                        else
                        {
                            if (!isLoadingNotifiedTrue)
                                Assert.Fail("IsLoading should not have notified when it's value was false, before it had notified that it was set to true");

                            if (isLoadingNotifiedFalse)
                                Assert.Fail("IsLoading notified more than one more time with a 'false' value");

                            isLoadingNotifiedFalse = true;
                        }
                    }
                };
            image.CreateImageRequested += (sender, e) =>
                {
                    imageRequested = e.ImageSize;
                    e.Handled = true;
                };

            string intialResponse = getPropertyValue(image);
            Assert.AreEqual(MockThumbnailImage.DefaultImageString, intialResponse, "Failed to return default string upon first request");
            Assert.IsNotNull(imageRequested, "Image was not requested after the property was queried");
            Assert.AreEqual(size, imageRequested, "Image requested size was not expected");
            Assert.IsFalse(thumbnailChangeNotified, "Thumbnail Property change should not have fired");
            Assert.IsTrue(isLoadingNotifiedTrue, "IsLoading notification should have fired");

            //Now set the image and ensure the property change notification occurrs
            const string mockImageData = "Image File Here";
            image.SetImage(size, mockImageData);
            Assert.AreEqual(mockImageData, getPropertyValue(image), "Failed to get expected image back after setting it");
            Assert.IsNotNull(thumbnailChangeNotified, "No property change was fired when setting the image");
            Assert.IsTrue(thumbnailChangeNotified, "Failed to notify property changed for thumbnail image");
            Assert.IsTrue(isLoadingNotifiedTrue, "Failed to notify that the image was loading");
            Assert.IsTrue(isLoadingNotifiedFalse, "Failed to notify that the image completed loading");
        }
    }
}
