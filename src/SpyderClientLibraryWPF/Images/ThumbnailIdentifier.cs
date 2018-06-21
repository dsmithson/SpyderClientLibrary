using Knightware.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETFX_CORE
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
#endif

namespace Spyder.Client.Images
{
    public class ThumbnailIdentifier : QFTThumbnailIdentifier
    {
        public ThumbnailIdentifier(string serverIP, string fileName)
            : base(serverIP, fileName)
        {
        }
    }
}
