﻿using Spyder.Client.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.IO;
using Spyder.Client.Net.Notifications;

namespace Spyder.Client
{
    public abstract class BindableSpyderClientBase : SpyderClientManagerBase
    {
        protected BindableSpyderClientBase(SpyderServerEventListenerBase serverEventListener, Func<string, Task<ISpyderClientExtended>> getSpyderClient) 
            :base(serverEventListener, getSpyderClient)
        {
        }
    }
}