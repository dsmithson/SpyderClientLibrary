using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Primitives
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        public static Rectangle Empty
        {
            get { return new Rectangle(0, 0, 0, 0); }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Top
        {
            get { return Y; }
        }

        public int Left
        {
            get { return X; }
        }

        public int Right
        {
            get { return X + Width; }
            set { Width = (value - X); }
        }

        public int Bottom
        {
            get { return Y + Height; }
            set { Height = (value - Y); }
        }

        public bool IsEmpty
        {
            get
            {
                if (X == 0 && Y == 0 && Width == 0 && Height == 0)
                    return true;
                else
                    return false;
            }
        }
        
        public Rectangle(int x, int y, int width, int height) : this()
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public Rectangle(Rectangle copyFrom) : this()
        {
            CopyFrom(copyFrom);
        }

        public void CopyFrom(Rectangle copyFrom)
        {
            this.X = copyFrom.X;
            this.Y = copyFrom.Y;
            this.Width = copyFrom.Width;
            this.Height = copyFrom.Height;
        }

        public void Offset(Point offset)
        {
            this.X += offset.X;
            this.Y += offset.Y;
        }

        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }

        public bool Contains(Point pt)
        {
            if (pt.X >= this.X && pt.X <= this.Right &&
                pt.Y >= this.Y && pt.Y <= this.Bottom)
                return true;
            else
                return false;
        }

        public bool Contains(Rectangle rect)
        {
            if (rect.X >= this.X && rect.Right <= this.Right &&
                rect.Y >= this.Y && rect.Bottom <= this.Bottom)
                return true;
            else
                return false;
        }
        
        public static Rectangle Offset(Rectangle rect, Point offset)
        {
            Rectangle response = new Rectangle(rect);
            response.Offset(offset);
            return response;
        }

        public static Rectangle Offset(Rectangle rect, int x, int y)
        {
            Rectangle response = new Rectangle(rect);
            response.Offset(x, y);
            return response;
        }

        public bool Equals(Rectangle other)
        {
            if (this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height)
                return true;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
                return false;

            Rectangle other = (Rectangle)obj;
            if (this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Rect: {0}, {1}, {2}, {3}", X, Y, Width, Height);
        }

        public static bool operator ==(Rectangle r1, Rectangle r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(Rectangle r1, Rectangle r2)
        {
            return !r1.Equals(r2);
        }
    }
}
