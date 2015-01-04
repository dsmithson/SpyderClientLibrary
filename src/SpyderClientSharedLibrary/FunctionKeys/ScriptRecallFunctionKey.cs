using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.FunctionKeys
{
    public class ScriptRecallFunctionKey : FunctionKey
    {
        private int scriptID;
        public int ScriptID
        {
            get { return scriptID; }
            set
            {
                if (scriptID != value)
                {
                    scriptID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int cueIndex;
        public int CueIndex
        {
            get { return cueIndex; }
            set
            {
                if (cueIndex != value)
                {
                    cueIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private string serverIP;
        public string ServerIP
        {
            get { return serverIP; }
            set
            {
                if (serverIP != value)
                {
                    serverIP = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(Common.IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as ScriptRecallFunctionKey;
            if (myCopyFrom != null)
            {
                this.ScriptID = myCopyFrom.ScriptID;
                this.CueIndex = myCopyFrom.CueIndex;
                this.ServerIP = myCopyFrom.ServerIP;
            }
        }
    }
}
