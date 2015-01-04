using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if DESKTOP
using System.Windows.Media.Imaging;
#elif NETFX_CORE
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Spyder.Client.Images
{
    public class ThumbnailImage : ThumbnailImageBase<ThumbnailIdentifier, BitmapImage>
    {
        protected override BitmapImage GetDefaultImage(ImageSize size)
        {
            return null;
        }
    }
}
