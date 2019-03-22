using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Scripting
{
    public class SourceElement : ScriptElement
    {
        public Content Content
        {
            get
            {
                if (Contents == null || Contents.Count < 1)
                    return null;
                else
                    return Contents[0];
            }
        }

        public string SourceName
        {
            get { return Content?.Name; }
        }

        public SourceElement()
        {
            this.LayerCount = 1;
        }
    }
}
