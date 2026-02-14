using Knightware.Primitives;
using Spyder.Client.Common;
using System;

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

    public class DrawingMixEffectUsage : PropertyChangedBase, IEquatable<DrawingMixEffectUsage>
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

        public DrawingMixEffectUsage()
        {

        }

        public DrawingMixEffectUsage(DrawingMixEffectUsage copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public void CopyFrom(DrawingMixEffectUsage copyFrom)
        {
            if (copyFrom == null)
                return;

            this.SourceID = copyFrom.sourceID;
            this.Label = copyFrom.Label;
            this.UsageType = copyFrom.UsageType;
        }

        public bool Equals(DrawingMixEffectUsage other)
        {
            if (other == null)
                return false;

            return SourceID == other.SourceID &&
                Label == other.Label &&
                UsageType == other.UsageType;
        }
    }
}
