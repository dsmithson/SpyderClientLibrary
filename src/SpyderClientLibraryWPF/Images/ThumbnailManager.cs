using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knightware.Diagnostics;
using Knightware.Net;
using Knightware.Primitives;
using Knightware.Threading;
using Spyder.Client.Net;

#if DESKTOP
using System.Windows.Media.Imaging;
#elif NETFX_CORE
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Spyder.Client.Images
{
    public delegate void ImageStreamGetRequestedHandler(object sender, ImageStreamGetRequestedEventArgs e);
    public delegate void ImageStreamSetRequestedHandler(object sender, ImageStreamSetRequestedEventArgs e);

    public partial class ThumbnailManager : QFTThumbnailManagerBase<ThumbnailIdentifier, BitmapImage, ThumbnailImage>
    {
        private readonly Dispatcher dispatcher = Dispatcher.Current;

        public event ImageStreamSetRequestedHandler ImageStreamSetRequested;
        protected void OnImageStreamSetRequested(ImageStreamSetRequestedEventArgs e)
        {
            if (ImageStreamSetRequested != null)
                ImageStreamSetRequested(this, e);
        }

        public event ImageStreamGetRequestedHandler ImageStreamGetRequested;
        protected void OnImageStreamGetRequested(ImageStreamGetRequestedEventArgs e)
        {
            if (ImageStreamGetRequested != null)
                ImageStreamGetRequested(this, e);
        }

        public ThumbnailManager(string localImagesFolder)
            : base(localImagesFolder)
        {
        }

        public virtual void RemoveViewImages(int viewID)
        {
            PerformImageListOperation((items) =>
                {
                    var keysToRemove = items.Keys.Where(key => key.ViewID == viewID).ToList();
                    foreach(var key in keysToRemove)
                    {
                        items.Remove(key);
                    }
                });
        }

        public virtual Task<string> GetDemoServerImageFolder()
        {
            return Task.FromResult(GetFolder("Demo"));
        }

        public override async Task<Stream> GetImageStreamAsync(ThumbnailIdentifier identifier, ImageSize targetSize)
        {
            //Allow the consuming class to have an opportunity to override the stream provided to the thumbnail engine.  This is initially being implemented
            //to allow the application to override requests to images for it's demo server instance.
            var args = new ImageStreamGetRequestedEventArgs() { Identifier = identifier };
            OnImageStreamGetRequested(args);
            await args.WaitForAllPendingDeferralsAsync();

            if (args.GetNativeImageStream != null)
            {
                return await args.GetNativeImageStream(identifier);
            }
            else
            {
                return await base.GetImageStreamAsync(identifier, targetSize);
            }
        }

        public override Task<bool> SetImageAsync(ThumbnailIdentifier identifier, Stream stream, FileProgressDelegate progress = null)
        {
            //Allow the consuming class to have an opportunity to override the stream provided to the thumbnail engine.  This is initially being implemented
            //to allow the application to override requests to images for it's demo server instance.
            var args = new ImageStreamSetRequestedEventArgs() { Identifier = identifier };
            OnImageStreamSetRequested(args);
            if (args.SetNativeImageStream != null)
            {
                return args.SetNativeImageStream(identifier, stream, progress);
            }
            else
            {
                return base.SetImageAsync(identifier, stream, progress);
            }
        }

        protected override async Task OnNativeResolutionAvailable(string serverIP, string fileName, Size nativeResolution)
        {
            //Find existing thumbnails that match our context
            var matches = new List<ThumbnailImage>();
            PerformImageListOperation((list) =>
            {
                foreach (var entry in list)
                {
                    if (string.Compare(entry.Key.ServerIP, serverIP, StringComparison.OrdinalIgnoreCase) == 0 &&
                        string.Compare(fileName, entry.Key.FileName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        matches.Add(entry.Value);
                    }
                }
            });

            await dispatcher.BeginInvoke((Action)(() =>
                {
                    foreach (var match in matches)
                    {
                        match.NativeResolution = nativeResolution;
                    }
                }));

            //Allow the base to update it's internal properties
            //Note: there is a hack here - the base will try to update the NativeResolution property on the thumbnails also,
            //which looks redundant, however it would cause an invalid operation exception since we're on a background thread.
            //Since I've updated the property above, this won't happen...
            await base.OnNativeResolutionAvailable(serverIP, fileName, nativeResolution);
        }

        private void GetNewsize(uint sourceWidth, uint sourceHeight, ImageSize targetSize, out uint targetWidth, out uint targetHeight)
        {
            double aspectRatio = (double)sourceWidth / (double)sourceHeight;
            uint maxWidthOrHeight = (uint)targetSize;

            //Apply default sizing with constraint on width
            targetWidth = Math.Min(sourceWidth, maxWidthOrHeight);
            targetHeight = (uint)Math.Round((double)targetWidth / aspectRatio);

            //Sanity check on new height
            if (targetHeight > maxWidthOrHeight)
            {
                //Resize by height constraint
                targetHeight = maxWidthOrHeight;
                targetWidth = (uint)Math.Round((double)maxWidthOrHeight * aspectRatio);
            }
        }
    }
}
