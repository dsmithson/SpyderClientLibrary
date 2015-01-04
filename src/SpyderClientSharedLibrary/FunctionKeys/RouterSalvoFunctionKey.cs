using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;

namespace Spyder.Client.FunctionKeys
{
    public class RouterSalvoFunctionKey : FunctionKey
    {
        private RouterSalvo salvo;
        public RouterSalvo Salvo
        {
            get { return salvo; }
            set
            {
                if (salvo != value)
                {
                    salvo = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as RouterSalvoFunctionKey;
            if (myCopyFrom != null)
            {
                this.Salvo = new RouterSalvo(myCopyFrom.Salvo);
            }
        }
    }
}
