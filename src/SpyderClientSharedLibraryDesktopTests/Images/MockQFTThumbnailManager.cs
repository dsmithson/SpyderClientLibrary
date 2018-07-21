using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Spyder.Client.Net;

namespace Spyder.Client.Images
{
    public delegate void ProcessImageStreamHandler<K, T>(object sender, ProcessImageStreamEventArgs<K, T> e);

    public class MockQFTThumbnailManager : QFTThumbnailManagerBase<QFTThumbnailIdentifier, string, MockThumbnailImage<QFTThumbnailIdentifier>>
    {
        public MockQFTThumbnailManager(string imageFolderRoot, GetRemoteImagePathHandler getRemoteImagePathHandler)
            : base(imageFolderRoot, getRemoteImagePathHandler)
        {
        }

        protected override Task<ProcessedImageResult> ScaleImageAsync(QFTThumbnailIdentifier identifier, ImageSize targetSize, System.IO.Stream nativeImageStream)
        {
            using (Bitmap nativeBitmap = (Bitmap)Bitmap.FromStream(nativeImageStream))
            {
                //Determine our scaled size with the correct A/R
                float aspectRatio = (float)nativeBitmap.Width / (float)nativeBitmap.Height;
                int scaledWidth = (int)targetSize;
                int scaledHeight = (int)Math.Round(scaledWidth / aspectRatio);
                if(scaledHeight > (int)targetSize)
                {
                    scaledHeight = (int)targetSize;
                    scaledWidth = (int)Math.Round(scaledHeight * aspectRatio);
                }

                using (Bitmap scaledBitmap = new Bitmap(scaledWidth, scaledHeight))
                {
                    using (Graphics g = Graphics.FromImage(scaledBitmap))
                    {
                        g.DrawImage(nativeBitmap,
                            new Rectangle(0, 0, scaledWidth, scaledHeight),
                            new Rectangle(0, 0, nativeBitmap.Width, nativeBitmap.Height),
                            GraphicsUnit.Pixel);
                    }

                    //save our scaled image to a stream
                    var scaledStream = new MemoryStream();
                    scaledBitmap.Save(scaledStream, System.Drawing.Imaging.ImageFormat.Png);
                    scaledStream.Seek(0, SeekOrigin.Begin);

                    return Task.FromResult(new ProcessedImageResult()
                    {
                        NativeSize = new Knightware.Primitives.Size(nativeBitmap.Width, nativeBitmap.Height),
                        ScaledSize = new Knightware.Primitives.Size(scaledBitmap.Width, scaledBitmap.Height),
                        ScaledStream = scaledStream
                    });
                }
            }
        }

        public event ProcessImageStreamHandler<QFTThumbnailIdentifier, string> ProcessImageStreamRequested;
        protected void OnProcessImageStreamRequested(ProcessImageStreamEventArgs<QFTThumbnailIdentifier, string> e)
        {
            if (ProcessImageStreamRequested != null)
                ProcessImageStreamRequested(this, e);
        }

        protected override Task<string> ProcessImageAsync(QFTThumbnailIdentifier identifier, System.IO.Stream fileStream)
        {
            var e = new ProcessImageStreamEventArgs<QFTThumbnailIdentifier, string>(identifier, fileStream);
            OnProcessImageStreamRequested(e);
            return Task.FromResult(e.Result);
        }
    }
}
