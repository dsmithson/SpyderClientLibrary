using Knightware.Net;
using Spyder.Client.Net.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Spyder.Client.Net
{
    public class SpyderClientTests : SpyderClientTestBase
    {
        public SpyderClientTests()
            : base((hardwareType, address) =>
                {
                    string localCacheRoot = Path.Combine(Path.GetTempPath(), "UnitTests");
                    string serverCacheFolderPath = Path.Combine(localCacheRoot, serverIP);
                    if (!Directory.Exists(serverCacheFolderPath))
                        Directory.CreateDirectory(serverCacheFolderPath);
                    
                    return Task.FromResult((ISpyderClient)(new SpyderClient(hardwareType, address, serverCacheFolderPath)));
                }
            )
        {
        }
    }
}
