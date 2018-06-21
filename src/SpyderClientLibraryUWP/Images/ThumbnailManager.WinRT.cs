using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knightware.Diagnostics;
using Knightware.Primitives;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace Spyder.Client.Images
{
    public partial class ThumbnailManager : QFTThumbnailManagerBase<ThumbnailIdentifier, BitmapImage, ThumbnailImage>
    {
        protected override async Task<ProcessedImageResult> ScaleImageAsync(ThumbnailIdentifier identifier, ImageSize targetSize, Stream nativeImageStream)
        {
            try
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(nativeImageStream.AsRandomAccessStream());

                uint scaledWidth, scaledHeight;
                GetNewsize(decoder.PixelWidth, decoder.PixelHeight, targetSize, out scaledWidth, out scaledHeight);

                var transform = new BitmapTransform() { ScaledWidth = scaledWidth, ScaledHeight = scaledHeight };
                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                var destinationStream = new MemoryStream();

                //Re-encode our image at the new size
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destinationStream.AsRandomAccessStream());
                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, scaledWidth, scaledHeight, 96, 96, pixelData.DetachPixelData());
                await encoder.FlushAsync();

                //Return our new scaled image stream
                destinationStream.Seek(0, SeekOrigin.Begin);
                return new ProcessedImageResult()
                {
                    ScaledStream = destinationStream,
                    NativeSize = new Size((int)decoder.PixelWidth, (int)decoder.PixelHeight),
                    ScaledSize = new Size((int)scaledWidth, (int)scaledHeight)
                };
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while scaling image file '{1}': {2}",
                    ex.GetType().Name, identifier, ex.Message);

                return null;
            }
        }

        protected override async Task<BitmapImage> ProcessImageAsync(ThumbnailIdentifier identifier, System.IO.Stream fileStream)
        {
            if (fileStream.Position != 0)
                fileStream.Seek(0, SeekOrigin.Begin);

            BitmapImage response = null;
            await dispatcher.BeginInvoke(async () =>
            {
                try
                {
                    response = new BitmapImage();
                    await response.SetSourceAsync(fileStream.AsRandomAccessStream());
                }
                catch (Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to load a bitmap from the provided stream.  Image identifier was {1}.  Message: {2}",
                        ex.GetType().Name, identifier.ToString(), ex.Message);
                }
            });
            return response;
        }

        public virtual void RemoveViewImages(int viewID)
        {
            PerformImageListOperation((items) =>
            {
                var keysToRemove = items.Keys.Where(key => key.ViewID == viewID).ToList();
                foreach (var key in keysToRemove)
                {
                    items.Remove(key);
                }
            });
        }
    }
}
