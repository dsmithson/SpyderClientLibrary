using Knightware.Primitives;
using Spyder.Client.Common;
using Spyder.Client.Models.StackupProviders;
using Spyder.Client.Net.DrawingData;

namespace Spyder.Client.Models
{
    public class RenderPixelSpace : RenderObject
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

        public int X
        {
            get { return rect.X; }
        }

        public int Y
        {
            get { return rect.Y; }
        }

        public int Width
        {
            get { return rect.Width; }
        }

        public int Height
        {
            get { return rect.Height; }
        }

        private Rectangle rect;
        public Rectangle Rect
        {
            get { return rect; }
            set
            {
                if (rect != value)
                {
                    rect = value;
                    OnPropertyChanged();
                    OnPropertyChanged("X");
                    OnPropertyChanged("Y");
                    OnPropertyChanged("Width");
                    OnPropertyChanged("Height");
                }
            }
        }

        private string background0;
        public string Background0
        {
            get { return background0; }
            set
            {
                if (background0 != value)
                {
                    background0 = value;
                    OnPropertyChanged();
                }
            }
        }

        private string background1;
        public string Background1
        {
            get { return background1; }
            set
            {
                if (background1 != value)
                {
                    background1 = value;
                    OnPropertyChanged();
                }
            }
        }

        private double background1Opacity;
        public double Background1Opacity
        {
            get { return background1Opacity; }
            set
            {
                if (background1Opacity != value)
                {
                    background1Opacity = value;
                    OnPropertyChanged();
                }
            }
        }

        private double scale = 1;
        public double Scale
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

        private double stackupProviderScale = 1;
        public double StackupProviderScale
        {
            get { return stackupProviderScale; }
            set
            {
                if (stackupProviderScale != value)
                {
                    stackupProviderScale = value;
                    OnPropertyChanged();
                }
            }
        }

        public void CopyFrom(DrawingData data, int pixelSpaceID, IStackupProvider stackupProvider = null)
        {
            var ps = data.GetPixelSpace(pixelSpaceID);
            if (ps == null)
                return;

            CopyFrom(ps, stackupProvider);
        }

        public void CopyFrom(PixelSpace pixelSpace, IStackupProvider stackupProvider = null)
        {
            this.ID = pixelSpace.ID;
            this.Rect = (stackupProvider == null ? pixelSpace.Rect : stackupProvider.GenerateOffsetRect(pixelSpace.Rect, pixelSpace.ID));
            this.StackupProviderScale = Rect.Width / ((double)pixelSpace.Rect.Width / pixelSpace.Scale);
            this.Scale = pixelSpace.Scale;
            this.ZIndex = pixelSpace.ID;

            var drawingPs = pixelSpace as DrawingPixelSpace;
            if (drawingPs == null)
            {
                this.Background0 = pixelSpace.NextBackgroundStill;
                this.Background1Opacity = 0;
            }
            else
            {
                this.Background1Opacity = ((255 - drawingPs.Layer1Transparency) / (double)255);

                if (drawingPs.NextBackgroundStillIsOnLayer1)
                {
                    this.Background0 = pixelSpace.LastBackgroundStill;
                    this.Background1 = pixelSpace.NextBackgroundStill;
                }
                else
                {
                    this.Background0 = pixelSpace.NextBackgroundStill;
                    this.Background1 = pixelSpace.LastBackgroundStill;
                }
            }
        }
    }
}
