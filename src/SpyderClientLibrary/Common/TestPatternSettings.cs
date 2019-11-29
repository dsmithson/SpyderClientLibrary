using Knightware.Primitives;

namespace Spyder.Client.Common
{
    /// <summary>
    /// Configuration options for loading a test pattern image using the Spyder external control API
    /// </summary>
    public class TestPatternSettings : PropertyChangedBase
    {
        /// <summary>
        /// Base pattern type to render
        /// </summary>
        public TestPatternType PatternType
        {
            get { return patternType; }
            set
            {
                if (patternType != value)
                {
                    patternType = value;
                    OnPropertyChanged();
                }
            }
        }
        private TestPatternType patternType;

        /// <summary>
        /// When enabled, a 1 pixel outline will be drawn around the test pattern using the foreground color
        /// </summary>
        public bool IsOutlineEnabled
        {
            get { return isOutlineEnabled; }
            set
            {
                if (isOutlineEnabled != value)
                {
                    isOutlineEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool isOutlineEnabled;

        /// <summary>
        /// When enabled, an ellipse will be drawn stretched to fill the test pattern raster using a 1 pixel wide line with the foreground color
        /// </summary>
        public bool IsCenterCircleEnabled
        {
            get { return isCenterCircleEnabled; }
            set
            {
                if (isCenterCircleEnabled != value)
                {
                    isCenterCircleEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool isCenterCircleEnabled;

        /// <summary>
        /// When enabled, an 'X' will be drawn to fill the test pattern image using the foreground color
        /// </summary>
        public bool IsCenterXEnabled
        {
            get { return isCenterXEnabled; }
            set
            {
                if (isCenterXEnabled != value)
                {
                    isCenterXEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool isCenterXEnabled;

        /// <summary>
        /// When enabled, a 32x32 pixel grid will be drawn to fill the test pattern image using the foreground color
        /// </summary>
        public bool IsGridEnabled
        {
            get { return isGridEnabled; }
            set
            {
                if (isGridEnabled != value)
                {
                    isGridEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool isGridEnabled;

        private Color backgroundColor = Color.FromArgb(255, 0, 0, 0);
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                if (backgroundColor != value)
                {
                    backgroundColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color foregroundColor = Color.FromArgb(255, 255, 255, 255);
        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                if (foregroundColor != value)
                {
                    foregroundColor = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
