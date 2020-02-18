using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DESKTOP
using System.Windows.Media.Imaging;
#elif NETCOREAPP
using System.Windows.Media.Imaging;
#elif NETFX_CORE
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Spyder.Client.Images
{
    public class DesignerThumbnailImage : ThumbnailImage
    {
        public override BitmapImage SmallImage
        {
            get { return GetDefaultImage(ImageSize.Small); }
        }

        public override BitmapImage MediumImage
        {
            get { return GetDefaultImage(ImageSize.Medium); }
        }

        public override BitmapImage LargeImage
        {
            get { return GetDefaultImage(ImageSize.Large); }
        }
    }
}
