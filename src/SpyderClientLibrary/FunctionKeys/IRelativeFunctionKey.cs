using System.Collections.Generic;

namespace Spyder.Client.FunctionKeys
{
    public interface IRelativeFunctionKey
    {
        bool IsAbsolute { get; set; }
        List<int> AbsoluteLayerIDs { get; set; }
    }
}
