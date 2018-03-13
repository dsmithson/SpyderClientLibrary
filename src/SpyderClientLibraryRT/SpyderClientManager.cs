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
    public class SpyderClientManager : SpyderClientManagerBase
    {
        /// <summary>
        /// Initializes a SpyderClientManager using a default folder path for local cache file storage
        /// </summary>
        public SpyderClientManager()
        : this("SpyderClient")
        {
        }

        /// <summary>
        /// Initializes a SpyderClientManager using a provided folder name within local storage for local file caching
        /// </summary>
        /// <param name="localCacheRoot">Folder name in local storage name for saving image and other cached files.  If the specified folder does not exist in local storage, it will be created.</param>
        public SpyderClientManager(string localCacheRoot)
        : base(
            (serverIP) =>
            {
                ISpyderClientExtended response = new SpyderClient(serverIP, localCacheRoot);

                return Task.FromResult(response);
            })
        {
        }

    }
}
