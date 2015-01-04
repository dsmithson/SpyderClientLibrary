using PCLStorage;
using Spyder.Client.Net;
using Spyder.Client.Net.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
