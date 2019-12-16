using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Knightware.Diagnostics;

namespace Spyder.Client.Images
{
    public partial class ThumbnailManager
    {
        protected override Task<ProcessedImageResult> ScaleImageAsync(ThumbnailIdentifier identifier, ImageSize targetSize, Stream nativeImageStream)
        {
            try
            {
                using(Bitmap native = (Bitmap)Bitmap.FromStream(nativeImageStream))
                {
                    uint scaledWidth, scaledHeight;
                    GetNewsize((uint)native.Width, (uint)native.Height, targetSize, out scaledWidth, out scaledHeight);

                    using(Bitmap scaled = new Bitmap(native, (int)scaledWidth, (int)scaledHeight))
                    {
                        var scaledStream = new MemoryStream();
                            scaled.Save(scaledStream, native.RawFormat);
                            scaledStream.Seek(0, SeekOrigin.Begin);

                            return Task.FromResult(new ProcessedImageResult()
                                {
                                    NativeSize = new Knightware.Primitives.Size(native.Width, native.Height),
                                    ScaledSize = new Knightware.Primitives.Size(scaled.Width, scaled.Height),
                                    ScaledStream = scaledStream
                                });
                    }
                }
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while scaling image file '{1}': {2}",
                    ex.GetType().Name, identifier, ex.Message);

                return Task.FromResult<ProcessedImageResult>(null);
            }
        }

        protected override Task<BitmapImage> ProcessImageAsync(ThumbnailIdentifier identifier, System.IO.Stream fileStream)
        {
            if (fileStream.Position != 0)
                fileStream.Seek(0, SeekOrigin.Begin);

            try
            {
                var response = new BitmapImage();
                response.BeginInit();
                response.StreamSource = fileStream;
                response.CacheOption = BitmapCacheOption.OnLoad;
                response.EndInit();
                response.Freeze();

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to load a bitmap from the provided stream.  Image identifier was {1}.  Message: {2}",
                    ex.GetType().Name, identifier.ToString(), ex.Message);

                return Task.FromResult<BitmapImage>(null);
            }
        }
    }
}
