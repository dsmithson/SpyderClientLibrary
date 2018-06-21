﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Spyder.Client.Common
{
    [TestClass]
    public class SystemDataV5Tests : SystemDataTestBase
    {
        protected override Stream GetTestSystemConfigStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version5.SystemConfiguration.xml");
        }

        protected override Stream GetTestScriptsStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version5.Scripts.xml");
        }
    }
}