using System.ComponentModel;

namespace Spyder.Client.Common
{
    public enum FieldRate
    {
        FR_60,
        NTSC,
        PAL,
        FR_48,
        FR_30,
        FR_29_97,
        FR_25,
        FR_24,
        FR_23_98,

        //Additional values added with Spyder-S
        FR_47_95 = 47,
        FR_50 = 50,
        FR_59_94 = 59,
        FR_96 = 96,
        FR_100 = 100,
        FR_119_88 = 119,
        FR_120 = 120,
        Unknown,
    }
}
