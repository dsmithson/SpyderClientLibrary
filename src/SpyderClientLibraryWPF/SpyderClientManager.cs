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
            (hardwareType, serverIP) =>
            {
                var serverCacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpyderClient");
                if (!Directory.Exists(serverCacheFolder))
                    Directory.CreateDirectory(serverCacheFolder);

                ISpyderClientExtended response = new SpyderClient(hardwareType, serverIP, serverCacheFolder);

                return Task.FromResult(response);
            })
        {
        }


        /// <summary>
        /// Initializes a SpyderClientManager using a provided file path for local cache file storage
        /// </summary>
        /// <param name="localCacheRoot">Directory root for saving image and other cached files.  If the specified directory does not exist, it will be created.</param>
        public SpyderClientManager(string localCacheRoot)
        : base(
            (hardwareType, serverIP) =>
            {
                string serverCacheFolderPath = Path.Combine(localCacheRoot, serverIP);
                if (!Directory.Exists(serverCacheFolderPath))
                    Directory.CreateDirectory(serverCacheFolderPath);
                
                ISpyderClientExtended response = new SpyderClient(hardwareType, serverIP, serverCacheFolderPath);

                return Task.FromResult(response);
            })
        {
        }
        
    }
}
