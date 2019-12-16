using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace Spyder.Client.Common
{
    [TestClass]
    public class ServerSettingsV4Tests : ServerSettingsTestBase
    {
        protected override Stream GetTestSystemSettingsStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version4.SystemSettings.xml");
        }
    }
}
