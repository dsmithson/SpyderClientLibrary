using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Primitives
{
    public struct Point : IEquatable<Point>
    {
        public static Point Empty
        {
            get { return new Point(0, 0); }
        }
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y) : this()
        {
            this.X = x;
            this.Y = y;
        }

        public bool Equals(Point other)
        {
            if (this.X == other.X && this.Y == other.Y)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;

            Point other = (Point)obj;
            if (this.X == other.X && this.Y == other.Y)
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
            return string.Format("X={0}, Y={1}", X, Y);
        }
    }
}
