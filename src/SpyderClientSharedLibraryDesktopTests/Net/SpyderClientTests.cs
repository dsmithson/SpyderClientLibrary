using Knightware.Net;
using Spyder.Client.Net.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PCLStorage;

namespace Spyder.Client.Net
{
    public class SpyderClientTests : ISpyderClientTestBase
    {
        public SpyderClientTests()
            : base(async (address) =>
                {
                    string localCacheRoot = Path.Combine(Path.GetTempPath(), "UnitTests");
                    string serverCacheFolderPath = Path.Combine(localCacheRoot, serverIP);
                    if (!Directory.Exists(serverCacheFolderPath))
                        Directory.CreateDirectory(serverCacheFolderPath);

                    var localCacheFolder = await FileSystem.Current.GetFolderFromPathAsync(serverCacheFolderPath);

                    return new SpyderClient(
                    async () => await SpyderServerEventListener.GetInstanceAsync(),
                    () => new TCPSocket(),
                    () => new UDPSocket(),
                    address,
                    localCacheFolder);
                }
            )
        {
        }
    }
}
