using Knightware.Primitives;
using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using Spyder.Client;

namespace Spyder.Client.Net.DrawingData
{
    public class DrawingMixEffect : PropertyChangedBase, IEquatable<DrawingMixEffect>
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

        public DrawingMixEffect()
        {

        }

        public DrawingMixEffect(DrawingMixEffect copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public void CopyFrom(DrawingMixEffect copyFrom)
        {
            if (copyFrom == null)
                return;

            ID = copyFrom.ID;
            Name = copyFrom.Name;
            BackgroundPixelSpaceId = copyFrom.BackgroundPixelSpaceId;
            Type = copyFrom.Type;
            TopIsFrozen = copyFrom.TopIsFrozen;
            BottomIsFrozen = copyFrom.BottomIsFrozen;
            TopSupportsFreeze = copyFrom.TopSupportsFreeze;
            BottomSupportsFreeze = copyFrom.BottomSupportsFreeze;
            TopContentName = copyFrom.TopContentName;
            TopContentThumbnail = copyFrom.TopContentThumbnail;
            BottomContentName = copyFrom.BottomContentName;
            BottomContentThumbnail = copyFrom.BottomContentThumbnail;
            TopContentOpacity = copyFrom.TopContentOpacity;
            Usages = new List<DrawingMixEffectUsage>(copyFrom.Usages);
        }

        public bool Equals(DrawingMixEffect other)
        {
            if (other == null)
                return false;

            return ID == other.ID &&
                   Name == other.Name &&
                   BackgroundPixelSpaceId == other.BackgroundPixelSpaceId &&
                   Type == other.Type &&
                   TopIsFrozen == other.TopIsFrozen &&
                   BottomIsFrozen == other.BottomIsFrozen &&
                   TopSupportsFreeze == other.TopSupportsFreeze &&
                   BottomSupportsFreeze == other.BottomSupportsFreeze &&
                   TopContentName == other.TopContentName &&
                   TopContentThumbnail == other.TopContentThumbnail &&
                   BottomContentName == other.BottomContentName &&
                   BottomContentThumbnail == other.BottomContentThumbnail &&
                   TopContentOpacity == other.TopContentOpacity &&
                   Usages.SequenceEqualSafe(other.Usages);
        }
    }
}
