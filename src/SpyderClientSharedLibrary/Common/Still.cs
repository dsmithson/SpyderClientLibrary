using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class Still : Register
    {
        private bool imageExistsAtServer;
        public bool ImageExistsAtServer
        {
            get { return imageExistsAtServer; }
            set
            {
                if (imageExistsAtServer != value)
                {
                    imageExistsAtServer = value;
                    OnPropertyChanged();
                }
            }
        }

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged();
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
                }
            }
        }

        private long fileSize;
        public long FileSize
        {
            get { return fileSize; }
            set
            {
                if (fileSize != value)
                {
                    fileSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public Still()
        {
        }

        public Still(IRegister copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public override void CopyFrom(IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            this.FileName = copyFrom.Name;

            var myCopyFrom = copyFrom as Still;
            if (myCopyFrom != null)
            {
                this.ImageExistsAtServer = myCopyFrom.ImageExistsAtServer;
                this.FileName = myCopyFrom.FileName;
                this.Width = myCopyFrom.Width;
                this.Height = myCopyFrom.Height;
                this.FileSize = myCopyFrom.FileSize;
            }
        }
    }
}
