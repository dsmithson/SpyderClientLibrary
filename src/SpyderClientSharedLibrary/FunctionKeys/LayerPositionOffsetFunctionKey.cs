using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.FunctionKeys
{
    public class LayerPositionOffsetFunctionKey : RelativeFunctionKey
    {
        private float hOffset;
        public float HOffset
        {
            get { return hOffset; }
            set
            {
                if (hOffset != value)
                {
                    hOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        private float vOffset;
        public float VOffset
        {
            get { return vOffset; }
            set
            {
                if (vOffset != value)
                {
                    vOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(Common.IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as LayerPositionOffsetFunctionKey;
            if (myCopyFrom != null)
            {
                this.HOffset = myCopyFrom.HOffset;
                this.VOffset = myCopyFrom.VOffset;
            }
        }
    }
}
