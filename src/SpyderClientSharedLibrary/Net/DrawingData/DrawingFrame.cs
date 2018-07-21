using Knightware.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;

namespace Spyder.Client.Net.DrawingData
{
    public class DrawingFrame : PropertyChangedBase
    {
        private int frameID;
        public int FrameID
        {
            get { return frameID; }
            set
            {
                if (frameID != value)
                {
                    frameID = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle frameAOR;
        public Rectangle FrameAOR
        {
            get { return frameAOR; }
            set
            {
                if (frameAOR != value)
                {
                    frameAOR = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// NOTE:  This property is no longer used starting in Spyder Studio 5.0.3.  Be careful about calculations depending on this property.
        /// </summary>
        public Rectangle PreviewAOR
        {
            get { return previewAOR; }
            set
            {
                if (previewAOR != value)
                {
                    previewAOR = value;
                    OnPropertyChanged();
                }
            }
        }
        private Rectangle previewAOR;

        private Rectangle programAOR;
        public Rectangle ProgramAOR
        {
            get { return programAOR; }
            set
            {
                if (programAOR != value)
                {
                    programAOR = value;
                    OnPropertyChanged();
                }
            }
        }

        private int renewalMasterFrameID;
        public int RenewalMasterFrameID
        {
            get { return renewalMasterFrameID; }
            set
            {
                if (renewalMasterFrameID != value)
                {
                    renewalMasterFrameID = value;
                    OnPropertyChanged();
                }
            }
        }

        private SpyderModels model;
        public SpyderModels Model
        {
            get { return model; }
            set
            {
                if (model != value)
                {
                    model = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
