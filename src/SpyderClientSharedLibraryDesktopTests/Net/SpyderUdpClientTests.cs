﻿using Knightware.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    [TestClass]
    public class SpyderUdpClientTests : SpyderClientTestBase
    {
        public SpyderUdpClientTests()
        : base((hardwareType, address) =>
        {
            ISpyderClient client = new SpyderUdpClient(serverIP);
            return Task.FromResult(client);
        })
        {
        }
    }
}
