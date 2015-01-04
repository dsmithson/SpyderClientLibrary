using Spyder.Client.Diagnostics;
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
        public int ViewID
        {
            get;
            private set;
        }

        public ThumbnailIdentifier(string serverIP, string fileName)
            : base(serverIP, fileName)
        {
            #if NETFX_CORE
            try
            {

                this.ViewID = ApplicationView.GetForCurrentView().Id;
            }
            catch(Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while getting current view ID: {1}", ex.GetType().Name, ex.Message);
                this.ViewID = -1;
            }
#endif
        }

        public ThumbnailIdentifier(int viewID, string serverIP, string fileName)
            : base(serverIP, fileName)
        {
            this.ViewID = viewID;
        }

        public override bool Equals(QFTThumbnailIdentifier other)
        {
            var compare = other as ThumbnailIdentifier;
            if (compare == null)
                return false;
            else if (compare.ViewID != this.ViewID)
                return false;
            else
                return base.Equals(other);
        }

        public override bool Equals(object other)
        {
            var compare = other as ThumbnailIdentifier;
            if (compare == null)
                return false;
            else if (compare.ViewID != this.ViewID)
                return false;
            else
                return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("View {0} - {1}", ViewID, base.ToString());
        }
    }
}
