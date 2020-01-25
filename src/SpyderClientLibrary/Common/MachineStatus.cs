namespace Spyder.Client.Common
{
    public class MachineStatus : PropertyChangedBase
    {
        public bool Cued
        {
            get { return cued; }
            set
            {
                if (cued != value)
                {
                    cued = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool cued = false;

        public bool AutoMode
        {
            get { return autoMode; }
            set
            {
                if (autoMode != value)
                {
                    autoMode = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool autoMode;

        public bool Playing
        {
            get { return playing; }
            set
            {
                if (playing != value)
                {
                    playing = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool playing = false;

        public bool Still
        {
            get { return still; }
            set
            {
                if (still != value)
                {
                    still = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool still;

        public bool ServoRefMissing
        {
            get { return servoRefMissing; }
            set
            {
                if (servoRefMissing != value)
                {
                    servoRefMissing = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool servoRefMissing = false;

        public bool TapeOut
        {
            get { return tapeOut; }
            set
            {
                if (tapeOut != value)
                {
                    tapeOut = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool tapeOut = false;

        public bool Local
        {
            get { return local; }
            set
            {
                if (local != value)
                {
                    local = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool local = false;

        public bool Standby
        {
            get { return standby; }
            set
            {
                if (standby != value)
                {
                    standby = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool standby = false;

        public bool Recording
        {
            get { return recording; }
            set
            {
                if (recording != value)
                {
                    recording = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool recording = false;

        public bool FastForwarding
        {
            get { return fastForwarding; }
            set
            {
                if (fastForwarding != value)
                {
                    fastForwarding = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool fastForwarding = false;

        public bool Rewinding
        {
            get { return rewinding; }
            set
            {
                if (rewinding != value)
                {
                    rewinding = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool rewinding = false;

        public bool Ejecting
        {
            get { return ejecting; }
            set
            {
                if (ejecting != value)
                {
                    ejecting = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool ejecting = false;

        public bool Stopped
        {
            get { return stopped; }
            set
            {
                if (stopped != value)
                {
                    stopped = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool stopped = false;

        public bool TapeDir
        {
            get { return tapeDir; }
            set
            {
                if (tapeDir != value)
                {
                    tapeDir = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool tapeDir = false;

        public bool Var
        {
            get { return var; }
            set
            {
                if (var != value)
                {
                    var = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool var = false;

        public bool Jog
        {
            get { return jog; }
            set
            {
                if (jog != value)
                {
                    jog = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool jog = false;

        public bool Shuttle
        {
            get { return shuttle; }
            set
            {
                if (shuttle != value)
                {
                    shuttle = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool shuttle = false;

        public bool TsoMode
        {
            get { return tsoMode; }
            set
            {
                if (tsoMode != value)
                {
                    tsoMode = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool tsoMode = false;

        public bool ServoLock
        {
            get { return servoLock; }
            set
            {
                if (servoLock != value)
                {
                    servoLock = value;
                    OnPropertyChanged();
                }
            }
        }
        protected bool servoLock = false;
    }
}
