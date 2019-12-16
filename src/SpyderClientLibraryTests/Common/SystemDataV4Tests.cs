using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace Spyder.Client.Common
{
    [TestClass]
    public class SystemDataV4Tests : SystemDataTestBase
    {
        protected override Stream GetTestSystemConfigStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version4.SystemConfiguration.xml");
        }

        protected override Stream GetTestScriptsStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version4.Scripts.xml");
        }
    }
}
