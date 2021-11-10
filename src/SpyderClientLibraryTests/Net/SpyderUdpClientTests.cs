using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    [TestClass]
    public class SpyderUdpClientTests : SpyderClientTestBase
    {
        public SpyderUdpClientTests()
        : base((hardwareType, address) =>
        {
            ISpyderClient client = new SpyderUdpClient(hardwareType, serverIP);
            return Task.FromResult(client);
        })
        {
        }
    }
}
