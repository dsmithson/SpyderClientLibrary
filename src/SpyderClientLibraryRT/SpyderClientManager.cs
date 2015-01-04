using Spyder.Client.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.IO;
using Spyder.Client.Net.Notifications;
using PCLStorage;

namespace Spyder.Client
{
    public class SpyderClientManager : SpyderClientManagerBase
    {
        public SpyderClientManager(IFolder localCacheRoot)
            : base(
            SpyderServerEventListener.Instance,

            async (serverIP) =>
            {
                var serverCacheFolder = await localCacheRoot.CreateFolderAsync(serverIP, CreationCollisionOption.OpenIfExists);

                ISpyderClientExtended response = new SpyderClient(
                SpyderServerEventListener.Instance,
                () => new TCPSocket(),
                () => new UDPSocket(),
                serverIP,
                serverCacheFolder);

                return response;
            })
        {
        }
    }
}
