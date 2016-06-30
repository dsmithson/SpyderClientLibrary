using Knightware.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Drawing
{
    [TestClass]
    public class BitmapHelperTests
    {
        [TestMethod]
        public void GenerateSolidColorBitmapTest()
        {
            const int imageWidth = 800;
            const int imageHeight = 600;
            var color = new Knightware.Primitives.Color(50, 100, 150);


            using (MemoryStream stream = new MemoryStream())
            {
                //Write image file
                BitmapHelper.GenerateSolidColorBitmap(stream, color, imageWidth, imageHeight);
                byte[] rawBytes = stream.ToArray();
                stream.Seek(0, SeekOrigin.Begin);

                //Now try to read the image into a bitmap that we can verify
                using (Bitmap bitmap = (Bitmap)Bitmap.FromStream(stream))
                {
                    Assert.AreEqual(imageWidth, bitmap.Width, "Bitmap width created was incorrect");
                    Assert.AreEqual(imageHeight, bitmap.Height, "Bitmap height created was incorrect");

                    //Verify all pixels
                    for (int y = 0; y < imageHeight; y++)
                    {
                        for (int x = 0; x < imageWidth; x++)
                        {
                            var pixel = bitmap.GetPixel(x, y);
                            Assert.AreEqual(color.R, pixel.R, "R value was incorrect at location {0}, {1}", x, y);
                            Assert.AreEqual(color.G, pixel.G, "G value was incorrect at location {0}, {1}", x, y);
                            Assert.AreEqual(color.B, pixel.B, "B value was incorrect at location {0}, {1}", x, y);
                        }
                    }
                }
            }
        }
    }
}
