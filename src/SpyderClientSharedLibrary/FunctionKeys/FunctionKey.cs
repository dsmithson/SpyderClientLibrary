using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;

namespace Spyder.Client.FunctionKeys
{
    public class FunctionKey : Register
    {
        

        public FunctionKey()
        {
        }

        public FunctionKey(IRegister copyFrom)
        {
            CopyFrom(copyFrom);
        }
    }
}
