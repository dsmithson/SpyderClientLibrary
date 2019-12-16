using System;

namespace Spyder.Client.Common
{
    [Flags]
    public enum ConnectorType
    {
        Auto = 0,
        Analog = 1,
        DVI = 2,
        HDMI = 4,
        DisplayPort = 8,
        SDI = 16,
        Composite = 32,
        SVideo = 64,

        // Convenience shortcuts
        AllX80 = SDI | HDMI | DVI | DisplayPort,
        AllX80ButSDI = HDMI | DVI | DisplayPort
    }

    public static class ConnectorTypeConverters
    {
        public static InputConnector ToInputConnector(this ConnectorType connectorType)
        {
            return (InputConnector)Enum.Parse(typeof(InputConnector), connectorType.ToString());
        }

        public static ConnectorType ToConnectorType(this InputConnector inputConnector)
        {
            if (inputConnector == InputConnector.HD15)
                return ConnectorType.Analog;

            return (ConnectorType)Enum.Parse(typeof(ConnectorType), inputConnector.ToString());
        }
    }
}
