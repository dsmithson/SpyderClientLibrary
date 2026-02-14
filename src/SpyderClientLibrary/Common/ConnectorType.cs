using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spyder.Client.Common
{
    /// <summary>
    /// Used to specify the 
    /// </summary>
    public enum ConnectorTypeUsage {  Unspecified, Input, Output, Router };

    public enum ConnectorType
    {
        Auto,
        Analog,
        Composite,
        DVI,
        DisplayPort,
        HDMI,
        SDI,
        SVideo
    }

    public static class ConnectorTypeExtensions
    {
        public static List<ConnectorType> GetValidConnectorTypes(this HardwareType hardwareType, ConnectorTypeUsage usage)
        {
            var response = new List<ConnectorType>();
            switch (hardwareType)
            {
                case HardwareType.SpyderS:
                case HardwareType.SpyderX80:
                    response.Add(ConnectorType.HDMI);
                    response.Add(ConnectorType.DisplayPort);
                    response.Add(ConnectorType.SDI);
                    break;
                default:
                    response.Add(ConnectorType.Analog);
                    response.Add(ConnectorType.DVI);
                    response.Add(ConnectorType.SDI);
                    response.Add(ConnectorType.Composite);
                    response.Add(ConnectorType.SVideo);
                    break;
            }

            //Auto is valid for inputs or router types
            if (usage != ConnectorTypeUsage.Output)
                response.Add(ConnectorType.Auto);

            return response;
        }

        public static bool IsValidConnectorTypeForHardware(this ConnectorType connectorType, HardwareType hardwareType, ConnectorTypeUsage usage)
        {
            return GetValidConnectorTypes(hardwareType, usage).Contains(connectorType);
        }
    }
}
