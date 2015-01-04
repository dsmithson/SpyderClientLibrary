using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class Router : PropertyChangedBase
    {
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

        private string routerType;
        public string RouterType
        {
            get { return routerType; }
            set
            {
                if (routerType != value)
                {
                    routerType = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private InputConnector connectorType;
        public InputConnector ConnectorType
        {
            get { return connectorType; }
            set
            {
                if (connectorType != value)
                {
                    connectorType = value;
                    OnPropertyChanged();
                }
            }
        }

        private int inputCount;
        public int InputCount
        {
            get { return inputCount; }
            set
            {
                if (inputCount != value)
                {
                    inputCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int outputCount;
        public int OutputCount
        {
            get { return outputCount; }
            set
            {
                if (outputCount != value)
                {
                    outputCount = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private bool serialRouter;
        public bool SerialRouter
        {
            get { return serialRouter; }
            set
            {
                if (serialRouter != value)
                {
                    serialRouter = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool levelControlledRouter;
        public bool LevelControlledRouter
        {
            get { return levelControlledRouter; }
            set
            {
                if (levelControlledRouter != value)
                {
                    levelControlledRouter = value;
                    OnPropertyChanged();
                }
            }
        }

        private int controlLevel;
        public int ControlLevel
        {
            get { return controlLevel; }
            set
            {
                if (controlLevel != value)
                {
                    controlLevel = value;
                    OnPropertyChanged();
                }
            }
        }

        private int levelCount;
        public int LevelCount
        {
            get { return levelCount; }
            set
            {
                if (levelCount != value)
                {
                    levelCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private string transportType;
        public string TransportType
        {
            get { return transportType; }
            set
            {
                if (transportType != value)
                {
                    transportType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string ipAddress;
        public string IPAddress
        {
            get { return ipAddress; }
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool ipRouter;
        public bool IPRouter
        {
            get { return ipRouter; }
            set
            {
                if (ipRouter != value)
                {
                    ipRouter = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, RouterPatch> patch = new Dictionary<int,RouterPatch>();
        public Dictionary<int, RouterPatch> Patch
        {
            get { return patch; }
            set
            {
                if (patch != value)
                {
                    patch = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<int> GetDownstreamRouterIDs()
        {
            List<int> response = new List<int>();
            foreach (RouterPatch patch in this.Patch.Values)
            {
                int downstreamRouter = patch.DownstreamRouterID;
                if (!response.Contains(downstreamRouter))
                    response.Add(downstreamRouter);
            }
            return response;
        }

        public int FindConnectedOutput(int downstreamRouterID, int downStreamInput)
        {
            foreach (RouterPatch patch in this.Patch.Values)
            {
                if (patch.DownstreamRouterID == downstreamRouterID && patch.DownstreamRouterInput == downStreamInput)
                    return patch.RouterOutput;
            }
            return -1;
        }

        /// <summary>
        /// Disconnects an output from any downstream patch association
        /// </summary>
        /// <param name="physicalOutput"></param>
        public void ClearRouterOutputPatch(int physicalOutput)
        {
            if (Patch.ContainsKey(physicalOutput))
                Patch.Remove(physicalOutput);
        }

        public void ClearRouterOutputPatch()
        {
            Patch.Clear();
        }

        /// <summary>
        /// Creates a patch assignment between a physical output and a downstream router's input
        /// </summary>
        public void SetRouterOutputPatch(int physicalOutput, int downstreamRouterID, int downstreamRouterInput)
        {
            if (physicalOutput < 0 || physicalOutput >= OutputCount)
                return;

            if (Patch == null)
                Patch = new Dictionary<int, RouterPatch>();

            if (Patch.ContainsKey(physicalOutput))
                Patch.Remove(physicalOutput);

            Patch.Add(physicalOutput, new RouterPatch()
            {
                DownstreamRouterID = downstreamRouterID,
                DownstreamRouterInput = downstreamRouterInput,
                RouterOutput = physicalOutput
            });
        }

        public void SetRouterOutputPatchUnity(int downstreamRouterID, int downstreamRouterInputCount)
        {
            Patch.Clear();
            int patchCount = Math.Min(this.OutputCount, downstreamRouterInputCount);
            for (int i = 0; i < patchCount; i++)
                SetRouterOutputPatch(i, downstreamRouterID, i);
        }

        public override string ToString()
        {
            if (Name != null && Name.Length > 0)
                return ID.ToString() + " - " + Name;
            else
                return ID.ToString() + " - No Name";
        }
    }
}
