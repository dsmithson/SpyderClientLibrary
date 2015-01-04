using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.FunctionKeys
{
    public interface IRelativeFunctionKey
    {
        bool IsAbsolute { get; set; }
        List<int> AbsoluteLayerIDs { get; set; }
    }
}
