using System.Collections.Generic;

namespace Spyder.Client.Common
{
    public class RouterSalvo : PropertyChangedBase
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

        private int routerID;
        public int RouterID
        {
            get { return routerID; }
            set
            {
                if (routerID != value)
                {
                    routerID = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// List of inputs, keyed by output
        /// </summary>
        public Dictionary<int, int> Salvos
        {
            get { return salvos; }
            set
            {
                if (salvos != value)
                {
                    salvos = value;
                    OnPropertyChanged();
                }
            }
        }
        private Dictionary<int, int> salvos = new Dictionary<int, int>();

        public RouterSalvo()
        {
        }

        public RouterSalvo(RouterSalvo copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public void CopyFrom(RouterSalvo copyFrom)
        {
            this.ID = copyFrom.ID;
            this.Name = copyFrom.Name;
            this.RouterID = copyFrom.RouterID;
            this.Salvos = new Dictionary<int, int>(copyFrom.Salvos);
        }
    }
}
