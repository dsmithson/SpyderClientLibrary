using Knightware.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.IO;
using Spyder.Client.Net.Notifications;
using Spyder.Client.Net;

namespace Spyder.Client
{
    public abstract class BindableSpyderClientBase : SpyderClientManagerBase
    {
        protected BindableSpyderClientBase(Func<string, Task<ISpyderClientExtended>> getSpyderClient) 
            :base(getSpyderClient)
        {
        }
    }
}
