using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class CommandKey : Register
    {
        /// <summary>
        /// Associated Script ID for this command key
        /// </summary>
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

        /// <summary>
        /// Determines if the command key uses a relative script
        /// </summary>
        private bool isRelative;
        public bool IsRelative
        {
            get { return isRelative; }
            set
            {
                if (isRelative != value)
                {
                    isRelative = value;
                    OnPropertyChanged();
                }
            }
        }

        private int cueCount;
        public int CueCount
        {
            get { return cueCount; }
            set
            {
                if (cueCount != value)
                {
                    cueCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public CommandKey()
        {
        }

        public CommandKey(IRegister copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public override void CopyFrom(IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as CommandKey;
            if (myCopyFrom != null)
            {
                this.ScriptID = myCopyFrom.ScriptID;
                this.IsRelative = myCopyFrom.IsRelative;
                this.CueCount = myCopyFrom.CueCount;
            }
        }
    }
}
