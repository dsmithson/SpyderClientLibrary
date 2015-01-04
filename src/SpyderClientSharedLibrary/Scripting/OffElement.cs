using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Scripting
{
    public class OffElement : ScriptElement
    {
        public int Duration 
        {
            get { return MixOffRate; } 
        }
    }
}
