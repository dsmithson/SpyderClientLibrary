using System.Collections.Generic;
using Knightware.Primitives;
using Spyder.Client.Common;

namespace Spyder.Client.Net.DrawingData
{
    public class DrawingMixEffect : PropertyChangedBase
    {
        private int id;
        public int ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        private int backgroundPixelSpaceId = -1;
        public int BackgroundPixelSpaceId
        {
            get { return backgroundPixelSpaceId; }
            set
            {
                if (backgroundPixelSpaceId != value)
                {
                    backgroundPixelSpaceId = value;
                    OnPropertyChanged();
                }
            }
        }

        private MixEffectType type;
        public MixEffectType Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool topIsFrozen;
        public bool TopIsFrozen
        {
            get { return topIsFrozen; }
            set
            {
                if (topIsFrozen != value)
                {
                    topIsFrozen = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool bottomIsFrozen;
        public bool BottomIsFrozen
        {
            get { return bottomIsFrozen; }
            set
            {
                if (bottomIsFrozen != value)
                {
                    bottomIsFrozen = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool topSupportsFreeze;
        public bool TopSupportsFreeze
        {
            get { return topSupportsFreeze; }
            set
            {
                if (topSupportsFreeze != value)
                {
                    topSupportsFreeze = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool bottomSupportsFreeze;
        public bool BottomSupportsFreeze
        {
            get { return bottomSupportsFreeze; }
            set
            {
                if (bottomSupportsFreeze != value)
                {
                    bottomSupportsFreeze = value;
                    OnPropertyChanged();
                }
            }
        }

        private string topContentName;
        public string TopContentName
        {
            get { return topContentName; }
            set
            {
                if (topContentName != value)
                {
                    topContentName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasContent));
                }
            }
        }

        private string topContentThumbnail;
        public string TopContentThumbnail
        {
            get { return topContentThumbnail; }
            set
            {
                if (topContentThumbnail != value)
                {
                    topContentThumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        private string bottomContentName;
        public string BottomContentName
        {
            get { return bottomContentName; }
            set
            {
                if (bottomContentName != value)
                {
                    bottomContentName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasContent));
                }
            }
        }

        private string bottomContentThumbnail;
        public string BottomContentThumbnail
        {
            get { return bottomContentThumbnail; }
            set
            {
                if (bottomContentThumbnail != value)
                {
                    bottomContentThumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        private double topContentOpacity;
        public double TopContentOpacity
        {
            get { return topContentOpacity; }
            set
            {
                if (topContentOpacity != value)
                {
                    topContentOpacity = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasContent => !string.IsNullOrEmpty(TopContentName) || !string.IsNullOrEmpty(BottomContentName);

        private List<DrawingMixEffectUsage> usages = new List<DrawingMixEffectUsage>();
        public List<DrawingMixEffectUsage> Usages
        {
            get { return usages; }
            set
            {
                if (usages != value)
                {
                    usages = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
