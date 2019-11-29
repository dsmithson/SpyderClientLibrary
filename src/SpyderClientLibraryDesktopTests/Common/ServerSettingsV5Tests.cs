using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace Spyder.Client.Common
{
    [TestClass]
    public class ServerSettingsV5Tests : ServerSettingsTestBase
    {
        public ServerSettingsV5Tests()
        {
            propertiesToIgnore = new string[]
                {
                    "AllowEventMulticast",
                    "HalType",
                    "Program",
                    "ConfigFPGA",
                    "AppInit",
                    "Monitor",
                    "ConsoleInterfaceEnabled",
                    "Height",
                    "AutoSelectHeight",
                    "UpdateBinariesIfPresent",
                    "VerifyPromsAfterProgram",
                    "UseRouterHAL",
                    "UseMachineHAL",
                    "UseFrontPanel",
                    "StringCommandAlerts",
                    "ShutdownAlertsAfterStartup",
                    "BackColor",
                    "RasterMeasureMaxVActive",
                    "RasterMeasureMaxVHoldoff",
                    "RasterMeasureMaxHActive",
                    "RasterMeasureMaxHHoldoff",
                    "IsGenlockEnabled",
                    "UpdateBinariesIfSame",
                    "PixelSpaceHorizontalOffset",
                    "PixelSpaceVerticalOffset",
                    "WriteAlertsToEventLog",
                    "CheckMultipleConnections",
                    "FrameID",
                    "DownStreamFrameCount",
                    "PatchSources1To1OnStartup",
                    "VerifyPromsAfterProgram",
                    "ConfigModeRasterColor",
                    "CreateVirtualPreviewLayers",
                    "UserDiagnosticMonitoringEnabled"
                };
        }

        protected override Stream GetTestSystemSettingsStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Spyder.Client.Resources.TestConfigs.Version5.FrameConfiguration.xml");
        }
    }
}
