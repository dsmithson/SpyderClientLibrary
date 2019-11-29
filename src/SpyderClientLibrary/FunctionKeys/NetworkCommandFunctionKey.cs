namespace Spyder.Client.FunctionKeys
{
    public enum NetworkProtocolType { TCP, UDP }

    public class NetworkCommandFunctionKey : FunctionKey
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

        private NetworkProtocolType protocolType;
        public NetworkProtocolType ProtocolType
        {
            get { return protocolType; }
            set
            {
                if (protocolType != value)
                {
                    protocolType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string serverAddress;
        public string ServerAddress
        {
            get { return serverAddress; }
            set
            {
                if (serverAddress != value)
                {
                    serverAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        private string command;
        public string Command
        {
            get { return command; }
            set
            {
                if (command != value)
                {
                    command = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(Common.IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as NetworkCommandFunctionKey;
            if (myCopyFrom != null)
            {
                this.Port = myCopyFrom.Port;
                this.ProtocolType = myCopyFrom.ProtocolType;
                this.ServerAddress = myCopyFrom.ServerAddress;
                this.Command = myCopyFrom.Command;
            }
        }
    }
}
