using PCLStorage;
using Knightware.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Net.Notifications;
using Spyder.Client.Net;
using System.IO;

namespace Spyder.Client
{
    public class SpyderClientManager : SpyderClientManagerBase
    {
        /// <summary>
        /// Initializes a SpyderClientManager using a default folder path for local cache file storage
        /// </summary>
        public SpyderClientManager()
        : base(
            async () => await SpyderServerEventListener.GetInstanceAsync(),

            async (serverIP) =>
            {
                var serverCacheFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("SpyderClient", CreationCollisionOption.OpenIfExists);

                ISpyderClientExtended response = new SpyderClient(
                async () => await SpyderServerEventListener.GetInstanceAsync(),
                () => new TCPSocket(),
                () => new UDPSocket(),
                serverIP,
                serverCacheFolder);

                return response;
            })
        {
        }


        /// <summary>
        /// Initializes a SpyderClientManager using a provided file path for local cache file storage
        /// </summary>
        /// <param name="localCacheRoot">Directory root for saving image and other cached files.  If the specified directory does not exist, it will be created.</param>
        public SpyderClientManager(string localCacheRoot)
        : base(
            async () => await SpyderServerEventListener.GetInstanceAsync(),

            async (serverIP) =>
            {
                string serverCacheFolderPath = Path.Combine(localCacheRoot, serverIP);
                if (!Directory.Exists(serverCacheFolderPath))
                    Directory.CreateDirectory(serverCacheFolderPath);

                var serverCacheFolder = await FileSystem.Current.GetFolderFromPathAsync(serverCacheFolderPath);

                ISpyderClientExtended response = new SpyderClient(
                async () => await SpyderServerEventListener.GetInstanceAsync(),
                () => new TCPSocket(),
                () => new UDPSocket(),
                serverIP,
                serverCacheFolder);

                return response;
            })
        {
        }

        public SpyderClientManager(IFolder localCacheRoot)
            : base(

            async () => await SpyderServerEventListener.GetInstanceAsync(),

            async (serverIP) =>
            {
                var serverCacheFolder = await localCacheRoot.CreateFolderAsync(serverIP, CreationCollisionOption.OpenIfExists);

                ISpyderClientExtended response = new SpyderClient(
                async () => await SpyderServerEventListener.GetInstanceAsync(),
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
