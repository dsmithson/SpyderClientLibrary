using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class Still : Register, IEquatable<Still>
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

        public bool Equals(Still other)
        {
            if(other == null)
                return false;
            else if(this.imageExistsAtServer != other.imageExistsAtServer)
                return false;
            else if(this.fileName != other.fileName)
                return false;
            else if(this.width != other.width)
                return false;
            else if(this.height != other.height)
                return false;
            else if(this.fileSize != other.fileSize)
                return false;
            else
                return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Still);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Still still1, Still still2)
        {
            if (((object)still1 == null) || ((object)still2) == null)
                return Object.Equals(still1, still2);

            return still1.Equals(still2);
        }

        public static bool operator !=(Still still1, Still still2)
        {
            if (((object)still1 == null) || ((object)still2) == null)
                return !Object.Equals(still1, still2);

            return !still1.Equals(still2);
        }
    }
}
