using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace Spyder.Client.Common
{
    [TestClass]
    public class SystemDataV5_2Tests : SystemDataTestBase
    {
        protected override Stream GetTestSystemConfigStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version5_2.SystemConfiguration.xml");
        }

        protected override Stream GetTestScriptsStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version5_2.Scripts.xml");
        }
    }
}
