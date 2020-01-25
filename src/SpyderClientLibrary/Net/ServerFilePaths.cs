using Spyder.Client.Common;

namespace Spyder.Client.Net
{
    public class ServerFilePaths
    {
        public string DataRoot { get; }
        public string ImageRoot { get; }

        public string ScriptsFilePath { get; }
        public string SystemConfigurationFilePath { get; }
        public string SystemSettingsFilePath { get; }

        public ServerFilePaths(string dataRoot, string imageRoot, string scriptsFilePath, string systemConfigurationFilePath, string systemSettingsFilePath)
        {
            this.DataRoot = dataRoot;
            this.ImageRoot = imageRoot;
            this.ScriptsFilePath = scriptsFilePath;
            this.SystemConfigurationFilePath = systemConfigurationFilePath;
            this.SystemSettingsFilePath = systemSettingsFilePath;
        }

        public ServerFilePaths(HardwareType hardwareType, VersionInfo version)
        {
            if (hardwareType == HardwareType.SpyderX80)
            {
                this.DataRoot = @"c:\SpyderData\V1";
                this.ImageRoot = @"c:\SpyderData\V1\Images";
                this.ScriptsFilePath = @"c:\SpyderData\V1\Scripts.xml";
                this.SystemConfigurationFilePath = @"c:\SpyderData\V1\SystemConfiguration.xml";
                this.SystemSettingsFilePath = @"c:\SpyderData\V1\FrameConfiguration.xml";
            }
            else
            {
                this.DataRoot = @"c:\Spyder";
                this.ImageRoot = @"c:\Spyder\Images";
                this.ScriptsFilePath = @"c:\Spyder\Scripts\Scripts.xml";
                this.SystemConfigurationFilePath = @"c:\Spyder\SystemConfiguration.xml";
                this.SystemSettingsFilePath = @"c:\Spyder\SystemSettings.xml";
            }

            //Spyder 200/300 store their system settings in a server relative path
            if (hardwareType == HardwareType.Spyder300)
            {
                this.SystemSettingsFilePath = $@"c:\Applications\SpyderServer\Version {version.Major}.{version.Minor}.{version.Build}\SystemSettings.xml";
            }
        }

        public static ServerFilePaths FromHardwareType(HardwareType hardwareType, VersionInfo version)
        {
            return new ServerFilePaths(hardwareType, version);
        }
    }
}
