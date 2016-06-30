﻿using Knightware.Net;
using Knightware.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knightware.Threading.Tasks;

namespace Spyder.Client.Net.Notifications
{
    public class SpyderServerEventListener : SpyderServerEventListenerBase
    {
        private static SpyderServerEventListener sharedInstance;
        private static readonly AsyncLock sharedInstanceLock = new AsyncLock();

        public static async Task<SpyderServerEventListener> GetInstanceAsync()
        {
            if (sharedInstance == null)
            {
                using (await sharedInstanceLock.LockAsync())
                {
                    if (sharedInstance == null)
                    {
                        sharedInstance = new SpyderServerEventListener();
                        await sharedInstance.StartupAsync();
                    }
                }
            }
            return sharedInstance;
        }

        [Obsolete("Use GetInstanceAsync static method instead")]
        public static SpyderServerEventListener Instance
        {
            get { return GetInstanceAsync().Result; }
        }

        private SpyderServerEventListener()
            : base(new UDPMulticastListener(), () => new GZipStreamDecompressor())
        {
        }
    }
}
