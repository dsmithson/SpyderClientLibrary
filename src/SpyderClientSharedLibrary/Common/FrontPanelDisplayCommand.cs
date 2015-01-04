using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public enum FrontPanelDisplayCommand
    {
        /// <summary>
        /// Initial Message to take over display
        /// </summary>
        Initializing,
        InitializingUSB,
        InitializingChannels,

        /// <summary>
        /// Status Message
        /// </summary>
        Status,
        FirmwareUpdateBegin,
        FirmwareUpdateComplete,
        ErasePromsBegin,
        ErasePromEnd,
        NetworkSuccess,
        NetworkError,
        MissingLicense,
        DeepPowerCycleRequired
    }
}
