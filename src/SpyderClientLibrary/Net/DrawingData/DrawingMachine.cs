using Spyder.Client.Common;

namespace Spyder.Client.Net.DrawingData
{
    public class DrawingMachine : PropertyChangedBase
    {
        private int port;
        public int Port
        {
            get { return port; }
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged();
                }
            }
        }

        private int lastPlayItemID;
        public int LastPlayItemID
        {
            get { return lastPlayItemID; }
            set
            {
                if (lastPlayItemID != value)
                {
                    lastPlayItemID = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeCode time = new TimeCode();
        public TimeCode Time
        {
            get { return time; }
            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged();
                }
            }
        }

        private MachineStatus status = new MachineStatus();
        public MachineStatus Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
