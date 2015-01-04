using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Scripting
{
    public class SourceElement : ScriptElement
    {
        public string SourceName
        {
            get
            {
                if (SourceNames == null || SourceNames.Count < 1)
                    return null;
                else
                    return SourceNames[0];
            }
        }

        public SourceElement()
        {
            this.LayerCount = 1;
        }
    }
}
