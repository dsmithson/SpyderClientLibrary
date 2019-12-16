﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    /// <summary>
    /// Result codes which can be returned from a Spyder server from a client request
    /// </summary>
    public enum ServerOperationResultCode
    {
        // First messages in this enum are returned values from the Spyder server
        //////////////////////////////////////////////////////////////////////////////

        Success = 0,
        DataNotAvailable = 1,
        InvalidHeader = 2,
        InvalidArgumentCount = 3,
        InvalidArgumentValue = 4,
        ExecutionError = 5,
        InvalidChecksum = 6,
        SuccessWithContinuation = 7,

        // Messages below are generated by the client class library
        ///////////////////////////////////////////////////////////////////////////////

        MissingCommandData = 100,
        ClientNotRunning,
        NoResponseFromServer,
        BadResponseFromServer
    }

}
