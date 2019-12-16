using Spyder.Client.Net.DrawingData;
using System;

namespace Spyder.Client.Net.Notifications
{
    public class DataObjectChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Source Spyder server IP address
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// Current DataObject version at Spyder server
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Flags enum of the data types changed witht the current version
        /// </summary>
        public DataType ChangedDataTypes { get; set; }

        public DataObjectChangedEventArgs()
        {
        }

        public DataObjectChangedEventArgs(string serverIP, int version, DataType changedDataTypes)
        {
            this.ServerIP = serverIP;
            this.Version = version;
            this.ChangedDataTypes = changedDataTypes;
        }
    }
}
