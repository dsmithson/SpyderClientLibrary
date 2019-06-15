using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public enum OutputMode 
    { 
        Normal, 
        Scaled, 
        OpMon, 
        SourceConfig, 
        PassiveLeft, 
        PassiveRight, 
        ActiveStereo, 
        SourceMon,

        //Added in X80.  
        //Note these are not in the enum order of X80, and explicit serialization/deserialization is required.  
        //Enum formats in correct byte order for X20 and X80 are below
        Multiviewer,
        Aux,
        UnscaledAux
    };

    /// <summary>
    /// Output mode enumerations byte-aligned with X80
    /// </summary>
    public enum OutputModeX80
    {
        Normal,
        Multiviewer,
        Scaled,
        Aux,
        UnscaledAux,
        OpMon,
        SourceMon,
        PassiveLeft,
        PassiveRight,
        ActiveStereo
    }

    /// <summary>
    /// Output mode enumerations byte-aligned with 200/300/X20 Spyder
    /// </summary>
    public enum OutputModeX20
    {
        Normal,
        Scaled,
        OpMon,
        SourceConfig,
        PassiveLeft,
        PassiveRight,
        ActiveStereo,
        SourceMon
    };
}
