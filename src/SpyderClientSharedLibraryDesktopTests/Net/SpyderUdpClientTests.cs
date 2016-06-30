using Knightware.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    [TestClass]
    public class SpyderUdpClientTests : ISpyderClientTestBase
    {
        public SpyderUdpClientTests()
        : base((address) =>
        {
            ISpyderClient client = new SpyderUdpClient(() => new UDPSocket(), serverIP);
            return Task.FromResult(client);
        })
        {
        }
    }
}
