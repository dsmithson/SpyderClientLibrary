using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Primitives
{
    public struct Thickness : IEquatable<Thickness>
    {
        public static Thickness Empty
        {
            get { return new Thickness(0); }
        }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public Thickness(double uniformThickness) : this()
        {
            this.Left = uniformThickness;
            this.Top = uniformThickness;
            this.Right = uniformThickness;
            this.Bottom = uniformThickness;
        }

        public Thickness(double left, double top, double right, double bottom) : this()
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public bool Equals(Thickness other)
        {
            if (this.Left == other.Left && 
                this.Right == other.Right && 
                this.Top == other.Top && 
                this.Bottom == other.Bottom)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Thickness))
                return false;

            Thickness other = (Thickness)obj;
            if (this.Left == other.Left &&
                this.Right == other.Right &&
                this.Top == other.Top &&
                this.Bottom == other.Bottom)
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
            return string.Format("Left={0}, Top={1}, Right={2}, Bottom={3}", Left, Top, Right, Bottom);
        }

        public static bool operator ==(Thickness t1, Thickness t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(Thickness t1, Thickness t2)
        {
            return !t1.Equals(t2);
        }
    }
}
