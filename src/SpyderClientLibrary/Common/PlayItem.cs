using System;

namespace Spyder.Client.Common
{
    public class PlayItem : Register, IEquatable<PlayItem>
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

        private TimeCode inTime;
        public TimeCode InTime
        {
            get { return inTime; }
            set
            {
                if (inTime != value)
                {
                    inTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeCode outTime;
        public TimeCode OutTime
        {
            get { return outTime; }
            set
            {
                if (outTime != value)
                {
                    outTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeCode duration;
        public TimeCode Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged();
                }
            }
        }

        private string defaultSource;
        public string DefaultSource
        {
            get { return defaultSource; }
            set
            {
                if (defaultSource != value)
                {
                    defaultSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private int preRollFrames;
        public int PreRollFrames
        {
            get { return preRollFrames; }
            set
            {
                if (preRollFrames != value)
                {
                    preRollFrames = value;
                    OnPropertyChanged();
                }
            }
        }

        private int rollOutFrames;
        public int RollOutFrames
        {
            get { return rollOutFrames; }
            set
            {
                if (rollOutFrames != value)
                {
                    rollOutFrames = value;
                    OnPropertyChanged();
                }
            }
        }

        private string clipName;
        public string ClipName
        {
            get { return clipName; }
            set
            {
                if (clipName != value)
                {
                    clipName = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte machineFlags;
        public byte MachineFlags
        {
            get { return machineFlags; }
            set
            {
                if (machineFlags != value)
                {
                    machineFlags = value;
                    OnPropertyChanged();
                }
            }
        }

        private string thumbnail;
        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                if (thumbnail != value)
                {
                    thumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        private FieldRate fieldRate;
        public FieldRate FieldRate
        {
            get { return fieldRate; }
            set
            {
                if (fieldRate != value)
                {
                    fieldRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public PlayItem()
        {
        }

        public PlayItem(IRegister copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public override void CopyFrom(IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as PlayItem;
            if (myCopyFrom != null)
            {
                this.ID = myCopyFrom.ID;
                this.InTime = myCopyFrom.InTime;
                this.OutTime = myCopyFrom.OutTime;
                this.Duration = myCopyFrom.Duration;
                this.DefaultSource = myCopyFrom.DefaultSource;
                this.PreRollFrames = myCopyFrom.PreRollFrames;
                this.RollOutFrames = myCopyFrom.RollOutFrames;
                this.ClipName = myCopyFrom.ClipName;
                this.MachineFlags = myCopyFrom.MachineFlags;
                this.Thumbnail = myCopyFrom.Thumbnail;
                this.FieldRate = myCopyFrom.FieldRate;
            }
        }

        public bool Equals(PlayItem other)
        {
            if (other == null)
                return false;
            else if (this.id != other.id)
                return false;
            else if (this.inTime != other.inTime)
                return false;
            else if (this.outTime != other.outTime)
                return false;
            else if (this.preRollFrames != other.preRollFrames)
                return false;
            else if (this.rollOutFrames != other.rollOutFrames)
                return false;
            else if (this.clipName != other.clipName)
                return false;
            else if (this.machineFlags != other.machineFlags)
                return false;
            else if (this.thumbnail != other.thumbnail)
                return false;
            else if (this.fieldRate != other.fieldRate)
                return false;
            else
                return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PlayItem);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(PlayItem playItem1, PlayItem playItem2)
        {
            if (((object)playItem1 == null) || ((object)playItem2) == null)
                return Object.Equals(playItem1, playItem2);

            return playItem1.Equals(playItem2);
        }

        public static bool operator !=(PlayItem playItem1, PlayItem playItem2)
        {
            if (((object)playItem1 == null) || ((object)playItem2) == null)
                return !Object.Equals(playItem1, playItem2);

            return !playItem1.Equals(playItem2);
        }
    }
}
