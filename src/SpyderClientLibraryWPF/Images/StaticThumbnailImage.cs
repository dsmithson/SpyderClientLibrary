using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knightware.Primitives;
using Knightware.Diagnostics;

#if DESKTOP
using System.Windows.Media.Imaging;
using System.Reflection;
#elif NETFX_CORE
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.ViewManagement;
#endif

namespace Spyder.Client.Images
{
    public class StaticThumbnailImage : ThumbnailImage
    {
        private static readonly Dictionary<int, StaticThumbnailImage> sourceImages = new Dictionary<int,StaticThumbnailImage>();
        public static StaticThumbnailImage Source 
        {
            get
            {
#if DESKTOP
                return GetImage(sourceImages, () =>
                    new StaticThumbnailImage(
                        "Spyder.Client.Assets._32x32.Source.png",
                        "Spyder.Client.Assets._128x128.Source.png",
                        "Spyder.Client.Assets._512x512.Source.png",
                        "Spyder.Client.Assets._512x512.Source.png"));
#elif NETFX_CORE
                return GetImage(sourceImages, () =>
                    new StaticThumbnailImage(
                        "ms-appx:///Assets/32x32/Source.png",
                        "ms-appx:///Assets/128x128/Source.png",
                        "ms-appx:///Assets/512x512/Source.png",
                        "ms-appx:///Assets/512x512/Source.png"));
#endif
            }
        }

        private static readonly Dictionary<int, StaticThumbnailImage> blackImages = new Dictionary<int,StaticThumbnailImage>();
        public static StaticThumbnailImage Black
        {
            get
            {
#if DESKTOP
                return GetImage(sourceImages, () =>
                    new StaticThumbnailImage("Spyder.Client.Assets._128x128.Black.png"));
#elif NETFX_CORE
                return GetImage(blackImages, () =>
                    new StaticThumbnailImage("ms-appx:///Assets/128x128/Black.png"));
#endif
            }
        }

        private static StaticThumbnailImage GetImage(Dictionary<int, StaticThumbnailImage> cache, Func<StaticThumbnailImage> create)
        {
            //Get view ID
            int viewID = 0;
            try
            {
#if NETFX_CORE
                viewID = ApplicationView.GetForCurrentView().Id;
#endif
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(null, TracingLevel.Warning, "{0} occurred while getting current view ID: {1}", ex.GetType().Name, ex.Message);
                viewID = -1;
            }

            //Look for existing item
            if (!cache.ContainsKey(viewID))
            {
                cache.Add(viewID, create());
            }

            return cache[viewID];
        }

        private readonly string uriExtraSmall;
        private readonly string uriSmall;
        private readonly string uriMedium;
        private readonly string uriLarge;
        private BitmapImage extraSmallImage;
        private BitmapImage smallImage;
        private BitmapImage mediumImage;
        private BitmapImage largeImage;

        public override BitmapImage ExtraSmallImage
        {
            get
            {
                if (extraSmallImage == null)
                    extraSmallImage = GetImage(uriExtraSmall);

                return extraSmallImage;
            }
        }

        public override BitmapImage SmallImage 
        { 
            get 
            {
                if (smallImage == null)
                    smallImage = GetImage(uriSmall);

                return smallImage;
            }
        }
        public override BitmapImage MediumImage
        {
            get
            {
                if (mediumImage == null)
                    mediumImage = GetImage(uriMedium);

                return mediumImage;
            }
        }
        public override BitmapImage LargeImage
        {
            get
            {
                if (largeImage == null)
                    largeImage = GetImage(uriLarge);

                return largeImage;
            }
        }

        public override Size NativeResolution { get; set; }

        public StaticThumbnailImage(string imageUri)
            : this(imageUri, imageUri, imageUri, imageUri)
        {
            
        }

        public StaticThumbnailImage(string extraSmallUri, string smallImageUri, string mediumImageUri, string largeImageUri)
        {
            this.uriExtraSmall = extraSmallUri;
            this.uriSmall = smallImageUri;
            this.uriMedium = mediumImageUri;
            this.uriLarge = largeImageUri;

            //Read in our 'native' resolution
            var image = GetImage(largeImageUri);
            NativeResolution = new Size(image.DecodePixelWidth, image.DecodePixelHeight);
        }

        protected override BitmapImage GetDefaultImage(ImageSize size)
        {
            if (size == ImageSize.Small)
                return GetImage(uriSmall);
            else if (size == ImageSize.Medium)
                return GetImage(uriMedium);
            else
                return GetImage(uriLarge);
        }

        private BitmapImage GetImage(string imageUri)
        {
#if DESKTOP
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(imageUri);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();

            return image;
#elif NETFX_CORE
            var image = new BitmapImage(new Uri(imageUri));
            return image;
#endif
        }
    }
}
