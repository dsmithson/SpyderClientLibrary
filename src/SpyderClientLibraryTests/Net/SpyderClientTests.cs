using System.IO;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    [TestClass]
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
