using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.FunctionKeys
{
    public class FreezeLayerKey : RelativeFunctionKey
    {
        private bool isFreezeEnabled;
        public bool IsFreezeEnabled
        {
            get { return isFreezeEnabled; }
            set
            {
                if (isFreezeEnabled != value)
                {
                    isFreezeEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(Common.IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as FreezeLayerKey;
            if (myCopyFrom != null)
            {
                this.IsFreezeEnabled = myCopyFrom.IsFreezeEnabled;
            }
        }
    }
}
