using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Primitives
{
    public struct Size : IEquatable<Size>
    {
        public static Size Empty
        {
            get { return new Size(0, 0); }
        }
        public int Width { get; set; }
        public int Height { get; set; }

        public Size(int width, int height)
            : this()
        {
            this.Width = width;
            this.Height = height;
        }

        public bool Equals(Size other)
        {
            if (this.Width == other.Width && this.Height == other.Height)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size))
                return false;

            Size other = (Size)obj;
            if (this.Width == other.Width && this.Height == other.Height)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Width={0}, Height={1}", Width, Height);
        }

        public static bool operator ==(Size s1, Size s2)
        {
            return s1.Equals(s2);
        }

        public static bool operator !=(Size s1, Size s2)
        {
            return !s1.Equals(s2);
        }
    }
}
