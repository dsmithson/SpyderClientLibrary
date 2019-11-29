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
