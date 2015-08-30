using Spyder.Client.Common;
using Knightware.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.DrawingData
{
    public enum OutputModuleType { SpyderDX4, SpyderUniversal, X20, URS, OutputBase };

    public class DrawingOutput : PropertyChangedBase
    {
        private int id;
        public int ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        private int hActive;
        public int HActive
        {
            get { return hActive; }
            set
            {
                if (hActive != value)
                {
                    hActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private int hTotal;
        public int HTotal
        {
            get { return hTotal; }
            set
            {
                if (hTotal != value)
                {
                    hTotal = value;
                    OnPropertyChanged();
                }
            }
        }

        private int vActive;
        public int VActive
        {
            get { return vActive; }
            set
            {
                if (vActive != value)
                {
                    vActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private int vTotal;
        public int VTotal
        {
            get { return vTotal; }
            set
            {
                if (vTotal != value)
                {
                    vTotal = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool interlaced;
        public bool Interlaced
        {
            get { return interlaced; }
            set
            {
                if (interlaced != value)
                {
                    interlaced = value;
                    OnPropertyChanged();
                }
            }
        }

        private float verticalRefresh;
        public float VerticalRefresh
        {
            get { return verticalRefresh; }
            set
            {
                if (verticalRefresh != value)
                {
                    verticalRefresh = value;
                    OnPropertyChanged();
                }
            }
        }

        private RotationMode rotation;
        public RotationMode Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    OnPropertyChanged();
                }
            }
        }

        private OutputModuleType outputType;
        public OutputModuleType OutputType
        {
            get { return outputType; }
            set
            {
                if (outputType != value)
                {
                    outputType = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<Rectangle> rectangles = new List<Rectangle>();
        public List<Rectangle> Rectangles
        {
            get { return rectangles; }
            set
            {
                if (rectangles != value)
                {
                    rectangles = value;
                    OnPropertyChanged();
                }
            }
        }

        private OutputMode outputMode;
        public OutputMode OutputMode
        {
            get { return outputMode; }
            set
            {
                if (outputMode != value)
                {
                    outputMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private HdcpLinkStatus hdcpStatus;
        public HdcpLinkStatus HdcpStatus
        {
            get { return hdcpStatus; }
            set
            {
                if (hdcpStatus != value)
                {
                    hdcpStatus = value;
                    OnPropertyChanged();
                }
            }
        }


        private Rectangle opMonProgramSource;
        public Rectangle OpMonProgramSource
        {
            get { return opMonProgramSource; }
            set
            {
                if (opMonProgramSource != value)
                {
                    opMonProgramSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle opMonProgramDest;
        public Rectangle OpMonProgramDest
        {
            get { return opMonProgramDest; }
            set
            {
                if (opMonProgramDest != value)
                {
                    opMonProgramDest = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle opMonPreviewSource;
        public Rectangle OpMonPreviewSource
        {
            get { return opMonPreviewSource; }
            set
            {
                if (opMonPreviewSource != value)
                {
                    opMonPreviewSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle opMonPreviewDest;
        public Rectangle OpMonPreviewDest
        {
            get { return opMonPreviewDest; }
            set
            {
                if (opMonPreviewDest != value)
                {
                    opMonPreviewDest = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle scaledSource;
        public Rectangle ScaledSource
        {
            get { return scaledSource; }
            set
            {
                if (scaledSource != value)
                {
                    scaledSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle scaledDest;
        public Rectangle ScaledDest
        {
            get { return scaledDest; }
            set
            {
                if (scaledDest != value)
                {
                    scaledDest = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
