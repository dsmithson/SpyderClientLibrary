using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spyder.Client.Images
{
    [TestClass]
    public class StaticThumbnailImageTests
    {
        [TestMethod]
        public void SourceImagesTest()
        {
            ImagesTest(StaticThumbnailImage.Source);
        }

        [TestMethod]
        public void BlackImagesTest()
        {
            ImagesTest(StaticThumbnailImage.Black);
        }

        private void ImagesTest(StaticThumbnailImage image)
        {
            Assert.IsNotNull(image, "Base image object was null");
            Assert.IsNotNull(image.ExtraSmallImage, "Extra small image was null");
            Assert.IsNotNull(image.SmallImage, "Small image was null");
            Assert.IsNotNull(image.MediumImage, "Medium image was null");
            Assert.IsNotNull(image.LargeImage, "Large image was null");
        }
    }
}
