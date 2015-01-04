using Spyder.Client.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.Notifications
{
    public class TraceLogMessageEventArgs : EventArgs
    {
        public string Address { get; set; }

        public TraceMessage Message { get; set; }

        public TraceLogMessageEventArgs()
        {

        }

        public TraceLogMessageEventArgs(string address, TraceMessage message)
        {
            this.Address = address;
            this.Message = message;
        }
    }
}
