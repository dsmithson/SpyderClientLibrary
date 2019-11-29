namespace Spyder.Client.Common
{
    public class RouterPatch : PropertyChangedBase
    {
        private int routerOutput;
        public int RouterOutput
        {
            get { return routerOutput; }
            set
            {
                if (routerOutput != value)
                {
                    routerOutput = value;
                    OnPropertyChanged();
                }
            }
        }

        private int downstreamRouterID;
        public int DownstreamRouterID
        {
            get { return downstreamRouterID; }
            set
            {
                if (downstreamRouterID != value)
                {
                    downstreamRouterID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int downstreamRouterInput;
        public int DownstreamRouterInput
        {
            get { return downstreamRouterInput; }
            set
            {
                if (downstreamRouterInput != value)
                {
                    downstreamRouterInput = value;
                    OnPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Output {0} patched to input {1} on router {2}", RouterOutput, DownstreamRouterInput, DownstreamRouterID);
        }
    }
}
