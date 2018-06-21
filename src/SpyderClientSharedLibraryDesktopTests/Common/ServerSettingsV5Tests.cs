﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spyder.Client.Common
{
    [TestClass]
    public class ServerSettingsV5Tests : ServerSettingsTestBase
    {
        protected override Stream GetTestSystemSettingsStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version5.FrameConfiguration.xml");
        }
    }
}