using System;
using System.ComponentModel;

namespace Spyder.Client.Common
{
    public enum SpyderModels
    {
        Spyder_380,
        Spyder_374,
        Spyder_371,
        Spyder_368,
        Spyder_365,
        Spyder_362,
        Spyder_353,
        Spyder_344,
        Spyder_335,
        Spyder_326,
        Spyder_308,
        Spyder_359,
        Spyder_3410,
        Spyder_240,
        Spyder_234,
        Spyder_231,
        Spyder_225,
        Spyder_222,
        Spyder_213,
        Spyder_207,
        Spyder_204,
        X20_1608,
        X20_0808,
        X80,
        Custom,
        SpyderS_HDMI_4x4,
        SpyderS_SDI_8x0,
        SpyderS_DP_4x4,
        SpyderS_SDI_16x0,
        SpyderS_SDI_4x4,
        SpyderS_SDI_0x8,
        SpyderS_SFP_4x4,
    }

    public enum Spyder200_300Models
    {
        Spyder_380,
        Spyder_374,
        Spyder_371,
        Spyder_368,
        Spyder_365,
        Spyder_362,
        Spyder_353,
        Spyder_344,
        Spyder_335,
        Spyder_326,
        Spyder_308,
        Spyder_359,
        Spyder_3410,
        Spyder_240,
        Spyder_234,
        Spyder_231,
        Spyder_225,
        Spyder_222,
        Spyder_213,
        Spyder_207,
        Spyder_204,
        Custom,
    }

    public enum SpyderX20Models
    {
        X20_1608,
        X20_0808,
        Custom
    }

    public enum SpyderSModels
    {
        Unknown = 0,
        HDMI_4x4 = 1,
        SDI_8x0 = 2,
        DP_4x4 = 3,
        SDI_16x0 = 4,
        SDI_4x4 = 5,
        SDI_0x8 = 6,
        SFP_4x4 = 7,
    }

    public static class SpyderModelsExtensions
    {
        public static SpyderModels Convert(this Spyder200_300Models model)
        {
            if(Enum.TryParse<SpyderModels>(model.ToString(), out var result))
            {
                return result;
            }
            return SpyderModels.Custom;
        }

        public static SpyderModels Convert(this SpyderSModels model)
        {
            return model switch
            {
                SpyderSModels.HDMI_4x4 => SpyderModels.SpyderS_HDMI_4x4,
                SpyderSModels.SDI_8x0 => SpyderModels.SpyderS_SDI_8x0,
                SpyderSModels.DP_4x4 => SpyderModels.SpyderS_DP_4x4,
                SpyderSModels.SDI_16x0 => SpyderModels.SpyderS_SDI_16x0,
                SpyderSModels.SDI_4x4 => SpyderModels.SpyderS_SDI_4x4,
                SpyderSModels.SDI_0x8 => SpyderModels.SpyderS_SDI_0x8,
                SpyderSModels.SFP_4x4 => SpyderModels.SpyderS_SFP_4x4,
                _ => SpyderModels.Custom,
            };
        }
    }
}
