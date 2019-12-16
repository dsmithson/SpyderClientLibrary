using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;

namespace Spyder.Client.Net.Notifications
{
    /// <summary>
    /// Contains information about a Spyder server on the network
    /// </summary>
    public class SpyderServerAnnounceInformation
    {
        public string ServerName { get; set; }
        public string Address { get; set; }
        public VersionInfo Version { get; set; }
        public int FrameID { get; set; }

        public bool IsVersion3OrHigher { get; set; }

        public HardwareType HardwareType { get; set; }
    }
}
