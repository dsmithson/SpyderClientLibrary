using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class PlayItem : Register
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
    }
}
