using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class CommandKey : Register, IEquatable<CommandKey>
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

        public bool Equals(CommandKey other)
        {
            if(other == null)
                return false;
            else if(this.scriptID != other.scriptID)
                return false;
            else if(this.isRelative != other.isRelative)
                return false;
            else if(this.cueCount != other.cueCount)
                return false;
            else
                return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandKey);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CommandKey key1, CommandKey key2)
        {
            if (((object)key1 == null) || ((object)key2) == null)
                return Object.Equals(key1, key2);

            return key1.Equals(key2);
        }

        public static bool operator !=(CommandKey key1, CommandKey key2)
        {
            if (((object)key1 == null) || ((object)key2) == null)
                return !Object.Equals(key1, key2);

            return !key1.Equals(key2);
        }
    }
}
