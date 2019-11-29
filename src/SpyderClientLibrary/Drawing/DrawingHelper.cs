using System.Drawing;

namespace Spyder.Client.Drawing
{
    public static class DrawingHelper
    {
        public static Color CalculateForegroundColor(Color backgroundColor)
        {
            double luminance = CalculateLuminance(backgroundColor);
            if (luminance > 128)
                return Color.FromArgb(0, 0, 0);
            else
                return Color.FromArgb(255, 255, 255);
        }

        public static double CalculateLuminance(Color color)
        {
            double luminance = (color.R * 0.3) + (color.G * 0.59) + (color.B * 0.11);
            return luminance;
        }
    }
}
