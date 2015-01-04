using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.Notifications
{
    public class SpyderServerEventListener : SpyderServerEventListenerBase
    {
        private static SpyderServerEventListener sharedInstance;

        public static SpyderServerEventListener Instance
        {
            get
            {
                if (sharedInstance == null)
                {
                    sharedInstance = new SpyderServerEventListener();
                    sharedInstance.Startup().Wait();
                }
                return sharedInstance;
            }
        }

        private SpyderServerEventListener()
            : base(new UDPMulticastListener(), () => new IO.GZipStreamDecompressor())
        {
        }
    }
}
