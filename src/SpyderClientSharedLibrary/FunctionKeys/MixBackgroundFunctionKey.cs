using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.FunctionKeys
{
    public class MixBackgroundFunctionKey : FunctionKey
    {
        private int duration;
        public int Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(Common.IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as MixBackgroundFunctionKey;
            if (myCopyFrom != null)
            {
                this.Duration = myCopyFrom.Duration;
            }
        }
    }
}
