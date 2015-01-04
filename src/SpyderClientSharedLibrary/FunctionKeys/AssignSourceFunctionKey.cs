using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;

namespace Spyder.Client.FunctionKeys
{
    public class AssignSourceFunctionKey : RelativeFunctionKey
    {
        private string sourceName;
        public string SourceName
        {
            get { return sourceName; }
            set
            {
                if (sourceName != value)
                {
                    sourceName = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as AssignSourceFunctionKey;
            if (myCopyFrom != null)
            {
                this.SourceName = myCopyFrom.SourceName;
            }
        }
    }
}
