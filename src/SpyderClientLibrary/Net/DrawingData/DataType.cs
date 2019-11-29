using System;

namespace Spyder.Client.Net.DrawingData
{
    [Flags]
    public enum DataType
    {
        None = 0x0000,
        Sources = 0x0001,
        BasicPresets = 0x0002,
        Treatments = 0x0004,
        Stills = 0x0008,
        FrameConfigs = 0x0010,
        Effects = 0x0020,
        PlayItems = 0x0040,
        InputConfigs = 0x0080,
        CommandKeys = 0x0100,
        FunctionKeys = 0x0200,
        Routers = 0x0400,
        ConsoleLayouts = 0x0800,
        OutputConfigs = 0x1000,
        PixelSpaces = 0x2000,
        DDRs = 0x4000,
        Devices = 0x8000,

        //Added in Spyder Studio (>5.0)
        Multiviewers = 0x10000,
        Edids = 0x20000,
        FrontPanel = 0x40000,

        RegisterListTypes = (DataType.Sources | DataType.Stills | DataType.Devices | DataType.PlayItems | DataType.CommandKeys | DataType.FunctionKeys | DataType.ConsoleLayouts | DataType.Treatments),
        All = 0xFFFFFF
    }
}
