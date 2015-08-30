using Knightware;
using Knightware.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class PixelSpace : PropertyChangedBase
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

        public Rectangle Rect
        {
            get { return new Rectangle(x, y, width, height); }
            set
            {
                X = (int)value.X;
                Y = (int)value.Y;
                Width = (int)value.Width;
                height = (int)value.Height;
            }
        }

        private int x;
        public int X
        {
            get { return x; }
            set
            {
                if (x != value)
                {
                    x = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Rect");
                }
            }
        }

        private int y;
        public int Y
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    y = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Rect");
                }
            }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Rect");
                }
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Rect");
                }
            }
        }

        private float scale;
        public float Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    OnPropertyChanged();
                }
            }
        }

        private string lastBackgroundStill;
        public string LastBackgroundStill
        {
            get { return lastBackgroundStill; }
            set
            {
                if (lastBackgroundStill != value)
                {
                    lastBackgroundStill = value;
                    OnPropertyChanged();
                }
            }
        }

        private string nextBackgroundStill;
        public string NextBackgroundStill
        {
            get { return nextBackgroundStill; }
            set
            {
                if (nextBackgroundStill != value)
                {
                    nextBackgroundStill = value;
                    OnPropertyChanged();
                }
            }
        }

        private int renewMasterFrameID;
        public int RenewMasterFrameID
        {
            get { return renewMasterFrameID; }
            set
            {
                if (renewMasterFrameID != value)
                {
                    renewMasterFrameID = value;
                    OnPropertyChanged();
                }
            }
        }

        private PixelSpaceStereoMode stereoMode;
        public PixelSpaceStereoMode StereoMode
        {
            get { return stereoMode; }
            set
            {
                if (stereoMode != value)
                {
                    stereoMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private TitleLocation layerTitleLocation;
        public TitleLocation LayerTitleLocation
        {
            get { return layerTitleLocation; }
            set
            {
                if (layerTitleLocation != value)
                {
                    layerTitleLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        private TitlePosition layerTitlePosition;
        public TitlePosition LayerTitlePosition
        {
            get { return layerTitlePosition; }
            set
            {
                if (layerTitlePosition != value)
                {
                    layerTitlePosition = value;
                    OnPropertyChanged();
                }
            }
        }

        private TitleSize layerTitleSize;
        public TitleSize LayerTitleSize
        {
            get { return layerTitleSize; }
            set
            {
                if (layerTitleSize != value)
                {
                    layerTitleSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color layerTitleForeground;
        public Color LayerTitleForeground
        {
            get { return layerTitleForeground; }
            set
            {
                if (layerTitleForeground != value)
                {
                    layerTitleForeground = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color layerTitleBackground;
        public Color LayerTitleBackground
        {
            get { return layerTitleBackground; }
            set
            {
                if (layerTitleBackground != value)
                {
                    layerTitleBackground = value;
                    OnPropertyChanged();
                }
            }
        }

        public PixelSpace()
        {
        }

        public PixelSpace(PixelSpace copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public virtual void CopyFrom(PixelSpace copyFrom)
        {
            this.Height = copyFrom.Height;
            this.ID = copyFrom.id;
            this.LastBackgroundStill = copyFrom.LastBackgroundStill;
            this.LayerTitleBackground = copyFrom.LayerTitleBackground;
            this.LayerTitleForeground = copyFrom.LayerTitleForeground;
            this.LayerTitlePosition = copyFrom.LayerTitlePosition;
            this.LayerTitleLocation = copyFrom.LayerTitleLocation;
            this.LayerTitleSize = copyFrom.LayerTitleSize;
            this.Name = copyFrom.Name;
            this.NextBackgroundStill = copyFrom.NextBackgroundStill;
            this.Rect = new Rectangle(copyFrom.Rect);
            this.RenewMasterFrameID = copyFrom.RenewMasterFrameID;
            this.Scale = copyFrom.Scale;
            this.StereoMode = copyFrom.StereoMode;
            this.Width = copyFrom.Width;
            this.X = copyFrom.X;
            this.Y = copyFrom.Y;
        }
    }
}
