using Knightware.Primitives;
using Spyder.Client.Common;

namespace Spyder.Client.Net.DrawingData
{
    public enum DrawingMixEffectUsageType
    {
        Background,
        ProgramLayer,
        PreviewLayer,
        Output,
        ProgramPixelSpace,
        PreviewPixelSpace,
    }

    public class DrawingMixEffectUsage : PropertyChangedBase
    {
        private int sourceID;
        public int SourceID
        {
            get { return sourceID; }
            set
            {
                if (sourceID != value)
                {
                    sourceID = value;
                    OnPropertyChanged();
                }
            }

        }

        private string label;
        public string Label
        {
            get { return label; }
            set
            {
                if (label != value)
                {
                    label = value;
                    OnPropertyChanged();
                }
            }
        }

        private DrawingMixEffectUsageType usageType;
        public DrawingMixEffectUsageType UsageType
        {
            get { return usageType; }
            set
            {
                if (usageType != value)
                {
                    usageType = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
