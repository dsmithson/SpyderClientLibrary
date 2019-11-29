using Knightware.Primitives;
using Spyder.Client.IO;
using Spyder.Client.IO.Serial;
using System.IO;
using System.Xml.Linq;

namespace Spyder.Client.Common
{
    public class ServerSettings
    {
        public BitDepth BitDepth { get; set; }

        public int MarshallingPort { get; set; }
        public int UdpEventPort { get; set; }
        public int UdpControlPort { get; set; }
        public int TcpControlPort { get; set; }
        public bool AllowEventMulticast { get; set; }
        public FieldRate FieldRate { get; set; }
        public HardwareType HardwareType { get; set; }
        public bool Program { get; set; }
        public bool ConfigFPGA { get; set; }
        public bool AppInit { get; set; }
        public bool Monitor { get; set; }
        public bool ConsoleEnabled { get; set; }
        public int Height { get; set; }
        public bool AutoSelectHeight { get; set; }
        public bool LoadBackgroundsOnStartup { get; set; }
        public bool UpdateBinariesIfPresent { get; set; }
        public bool VerifyPromsAfterProgram { get; set; }
        public int BasicPresetOnStartup { get; set; }
        public int CommandKeyScriptIDOnStartup { get; set; }
        public bool UseRouterHAL { get; set; }
        public bool UseMachineHAL { get; set; }
        public bool UseFrontPanel { get; set; }
        public bool StringCommandAlerts { get; set; }
        public bool ShutdownAlertsAfterStartup { get; set; }
        public Color BackColor { get; set; }
        public ExternalControlPort SerialControlPort { get; set; }
        public ExternalControlProtocol SerialControlProtocol { get; set; }
        public StopBits SerialControlStopBits { get; set; }
        public int SerialControlBaudRate { get; set; }
        public Parity SerialControlParity { get; set; }
        public int SerialOdeticsCommandKeyPage { get; set; }
        public bool CompressNetworkData { get; set; }
        public int InputTimingChangePollInterval { get; set; }
        public int RasterMeasureMaxVActive { get; set; }
        public int RasterMeasureMaxVHoldoff { get; set; }
        public int RasterMeasureMaxHActive { get; set; }
        public int RasterMeasureMaxHHoldoff { get; set; }
        public Color ConfigModeRasterColor { get; set; }
        public string MasterSourceMasterIP { get; set; }
        public string SlaveSourceMasterIP { get; set; }
        public string MasterSR208IP { get; set; }
        public string SlaveSR208IP { get; set; }
        public CalcType ScriptDefaultSpeedType { get; set; }
        public CalcType ScriptDefaultMotionType { get; set; }
        public bool GenerateSourceMonOverlays { get; set; }
        public bool SwitchRouterWhenCrosspointAlreadySet { get; set; }
        public HdcpMode HdcpMode { get; set; }
        public bool CreateVirtualPreviewLayers { get; set; }
        public bool LockFrontPanelOnStartup { get; set; }
        public bool UseUserConfigsForAutoSyncLookups { get; set; }
        public bool UserDiagnosticMonitoringEnabled { get; set; }
        public bool PerformRasterAdjustOnAutoSync { get; set; }
        public bool EnableAutoSyncHardwareAssist { get; set; }
        public bool IsGenlockEnabled { get; set; }
        public bool UpdateBinariesIfSame { get; set; }
        public bool LayersVisibleOnStartup { get; set; }
        public int CommandKeyCueIDOnStartup { get; set; }
        public int PixelSpaceHorizontalOffset { get; set; }
        public int PixelSpaceVerticalOffset { get; set; }
        public bool WriteAlertsToEventLog { get; set; }
        public bool CheckMultipleConnections { get; set; }
        public int FrameID { get; set; }
        public int DownStreamFrameCount { get; set; }
        public bool PatchSources1To1OnStartup { get; set; }

        public ServerSettings()
        {
        }

        public virtual bool Load(Stream systemSettingsStream)
        {
            return Load(systemSettingsStream, new SpyderXmlDeserializer());
        }

        protected virtual bool Load(Stream systemSettingsStream, SpyderXmlDeserializer deserializer)
        {
            if (systemSettingsStream == null || deserializer == null)
                return false;

            //Load Document
            XDocument xml = deserializer.GetXDocument(systemSettingsStream);
            if (xml == null)
                return false;

            //Deserialize properties
            XElement root = xml.Root;
            this.MarshallingPort = deserializer.Read(root, "MarshallingPort", -1);
            this.UdpEventPort = deserializer.Read(root, "UdpEventPort", -1);
            this.UdpControlPort = deserializer.Read(root, "UdpControlPort", -1);
            this.TcpControlPort = deserializer.Read(root, "TcpControlPort", -1);
            this.AllowEventMulticast = deserializer.Read(root, "AllowEventMulticast", false);
            this.FieldRate = deserializer.ReadEnum(root, "FieldRate", FieldRate.NTSC);
            this.HardwareType = deserializer.ReadEnum(root, "HalType", HardwareType.Virtual);
            this.Program = deserializer.Read(root, "Program", false);
            this.ConfigFPGA = deserializer.Read(root, "ConfigFPGA", false);
            this.AppInit = deserializer.Read(root, "AppInit", false);
            this.Monitor = deserializer.Read(root, "Monitor", false);
            this.ConsoleEnabled = deserializer.Read(root, "ConsoleInterfaceEnabled", false);
            this.Height = deserializer.Read(root, "Height", -1);
            this.AutoSelectHeight = deserializer.Read(root, "AutoSelectHeight", false);
            this.LoadBackgroundsOnStartup = deserializer.Read(root, "LoadBackgroundsOnStartup", true);
            this.UpdateBinariesIfPresent = deserializer.Read(root, "UpdateBinariesIfPresent", true);
            this.VerifyPromsAfterProgram = deserializer.Read(root, "VerifyPromsAfterProgram", true);
            this.BasicPresetOnStartup = deserializer.Read(root, "BasicPresetOnStartup", -1);
            this.CommandKeyScriptIDOnStartup = deserializer.Read(root, "CommandKeyScriptIDOnStartup", -1);
            this.UseRouterHAL = deserializer.Read(root, "UseRouterHAL", true);
            this.UseMachineHAL = deserializer.Read(root, "UseMachineHAL", true);
            this.UseFrontPanel = deserializer.Read(root, "UseFrontPanel", true);
            this.StringCommandAlerts = deserializer.Read(root, "StringCommandAlerts", true);
            this.ShutdownAlertsAfterStartup = deserializer.Read(root, "ShutdownAlertsAfterStartup", true);
            this.BackColor = deserializer.Read(root, "BackColor", new Color());
            this.SerialControlPort = deserializer.ReadEnum(root, "SerialControlPort", ExternalControlPort.None);
            this.SerialControlProtocol = deserializer.ReadEnum(root, "SerialControlProtocol", ExternalControlProtocol.StringCommands);
            this.SerialControlStopBits = deserializer.ReadEnum(root, "SerialControlStopBits", StopBits.None);
            this.SerialControlBaudRate = deserializer.Read(root, "SerialControlBaudRate", -1);
            this.SerialControlParity = deserializer.ReadEnum(root, "SerialControlParity", Parity.None);
            this.SerialOdeticsCommandKeyPage = deserializer.Read(root, "SerialOdeticsCommandKeyPage", -1);
            this.CompressNetworkData = deserializer.Read(root, "CompressNetworkData", false);
            this.InputTimingChangePollInterval = deserializer.Read(root, "InputTimingChangePollInterval", -1);
            this.RasterMeasureMaxVActive = deserializer.Read(root, "RasterMeasureMaxVActive", -1);
            this.RasterMeasureMaxVHoldoff = deserializer.Read(root, "RasterMeasureMaxVHoldoff", -1);
            this.RasterMeasureMaxHActive = deserializer.Read(root, "RasterMeasureMaxHActive", -1);
            this.RasterMeasureMaxHHoldoff = deserializer.Read(root, "RasterMeasureMaxHHoldoff", -1);
            this.ConfigModeRasterColor = deserializer.Read(root, "ConfigModeRasterColor", new Color(255, 255, 255));
            this.MasterSourceMasterIP = deserializer.Read(root, "MasterSourceMasterIP", "172.16.3.1");
            this.SlaveSourceMasterIP = deserializer.Read(root, "SlaveSourceMasterIP", "172.16.3.2");
            this.MasterSR208IP = deserializer.Read(root, "MasterSR208IP", "172.16.1.253");
            this.SlaveSR208IP = deserializer.Read(root, "SlaveSR208IP", "172.16.1.254");
            this.ScriptDefaultSpeedType = deserializer.ReadEnum(root, "ScriptDefaultSpeedType", CalcType.Hermite);
            this.ScriptDefaultMotionType = deserializer.ReadEnum(root, "ScriptDefaultMotionType", CalcType.Bezier);
            this.GenerateSourceMonOverlays = deserializer.Read(root, "GenerateSourceMonOverlays", false);
            this.SwitchRouterWhenCrosspointAlreadySet = deserializer.Read(root, "SwitchRouterWhenCrosspointAlreadySet", true);
            this.HdcpMode = deserializer.ReadEnum(root, "HDCPMode", HdcpMode.Disabled);
            this.CreateVirtualPreviewLayers = deserializer.Read(root, "CreateVirtualPreviewLayers", false);
            this.LockFrontPanelOnStartup = deserializer.Read(root, "LockFrontPanelOnStartup", false);
            this.UseUserConfigsForAutoSyncLookups = deserializer.Read(root, "UseUserConfigsForAutoSyncLookups", false);
            this.UserDiagnosticMonitoringEnabled = deserializer.Read(root, "UserDiagnosticMonitoringEnabled", false);
            this.PerformRasterAdjustOnAutoSync = deserializer.Read(root, "PerformRasterAdjustOnAutoSync", false);
            this.EnableAutoSyncHardwareAssist = deserializer.Read(root, "EnableAutoSyncHardwareAssist", false);
            this.IsGenlockEnabled = deserializer.Read(root, "IsGenlockEnabled", false);
            this.UpdateBinariesIfSame = deserializer.Read(root, "UpdateBinariesIfSame", false);
            this.LayersVisibleOnStartup = deserializer.Read(root, "LayersVisibleOnStartup", false);
            this.CommandKeyCueIDOnStartup = deserializer.Read(root, "CommandKeyCueIDOnStartup", -1);
            this.PixelSpaceHorizontalOffset = deserializer.Read(root, "PixelSpaceHorizontalOffset", 0);
            this.PixelSpaceVerticalOffset = deserializer.Read(root, "PixelSpaceVerticalOffset", 0);
            this.WriteAlertsToEventLog = deserializer.Read(root, "WriteAlertsToEventLog", false);
            this.CheckMultipleConnections = deserializer.Read(root, "CheckMultipleConnections", true);
            this.FrameID = deserializer.Read(root, "FrameID", 0);
            this.DownStreamFrameCount = deserializer.Read(root, "DownStreamFrameCount", 0);
            this.PatchSources1To1OnStartup = deserializer.Read(root, "PatchSources1To1OnStartup", false);

            return true;
        }
    }
}
