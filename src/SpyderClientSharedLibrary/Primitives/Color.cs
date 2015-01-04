using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Primitives
{
    public struct Color : IEquatable<Color>
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public Color(byte r, byte g, byte b)
            : this(255, r, g, b)
        {
        }

        public Color(byte a, byte r, byte g, byte b) : this()
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public Color(Color copyFrom) : this()
        {
            CopyFrom(copyFrom);
        }

        public Color(string argb) : this()
        {
            Parse(argb);
        }

        public void CopyFrom(Color copyFrom)
        {
            this.A = copyFrom.A;
            this.R = copyFrom.R;
            this.G = copyFrom.G;
            this.B = copyFrom.B;
        }

        public void Parse(string argb)
        {
            if (string.IsNullOrEmpty(argb))
                throw new ArgumentException("Invalid ARGB string");

            string[] parts = argb.Split(',');
            if (parts.Length != 3 && parts.Length != 4)
                throw new ArgumentException("Invalid ARGB string");

            int index = 0;
            if (parts.Length == 4)
            {
                this.A = byte.Parse(parts[index++]);
            }

            this.R = byte.Parse(parts[index++]);
            this.G = byte.Parse(parts[index++]);
            this.B = byte.Parse(parts[index++]);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Color))
                return false;

            Color other = (Color)obj;
            if (other.R != this.R || other.G != this.G || other.B != this.B || other.A != this.A)
                return false;

            return true;
        }

        public bool Equals(Color other)
        {
            if (other.R != this.R || other.G != this.G || other.B != this.B || other.A != this.A)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("R={0}, G={1}, B={2}", R, G, B);
        }

        public static bool operator ==(Color r1, Color r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(Color r1, Color r2)
        {
            return !r1.Equals(r2);
        }

        public static Color FromRgb(byte r, byte g, byte b)
        {
            return new Color(255, r, g, b);
        }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(a, r, g, b);
        }
    }
}
