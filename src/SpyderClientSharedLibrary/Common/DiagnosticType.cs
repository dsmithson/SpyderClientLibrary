using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public enum DiagnosticType
    {
        FrameLock, 
        Temperature, 
        FirmwareMatch, 
        FirmwareExecution, 
        Calibration, 
        ModuleType, 
        VideoConnection, 
        DataConnection, 
        StatusRefresh, 
        FrameRate, 
        Power
    }
}
