using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.DrawingData
{
    public class DrawingPixelSpace : PixelSpace
    {
        private bool nextBackgroundStillIsOnLayer1;
        public bool NextBackgroundStillIsOnLayer1
        {
            get { return nextBackgroundStillIsOnLayer1; }
            set
            {
                if (nextBackgroundStillIsOnLayer1 != value)
                {
                    nextBackgroundStillIsOnLayer1 = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private byte layer1Transparency;
        public byte Layer1Transparency
        {
            get { return layer1Transparency; }
            set
            {
                if (layer1Transparency != value)
                {
                    layer1Transparency = value;
                    OnPropertyChanged();
                }
            }
        }

        public DrawingPixelSpace()
        {
        }

        public DrawingPixelSpace(PixelSpace copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public override void CopyFrom(PixelSpace copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as DrawingPixelSpace;
            if (myCopyFrom != null)
            {
                this.NextBackgroundStillIsOnLayer1 = myCopyFrom.NextBackgroundStillIsOnLayer1;
                this.Layer1Transparency = myCopyFrom.Layer1Transparency;
            }
        }
    }
}
