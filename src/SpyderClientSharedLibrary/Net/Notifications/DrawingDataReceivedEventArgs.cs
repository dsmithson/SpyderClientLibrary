using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.Notifications
{
    public class DrawingDataReceivedEventArgs : EventArgs
    {
        public string ServerIP { get; set; }

        public DrawingData.DrawingData DrawingData { get; set; }

        public DrawingDataReceivedEventArgs(string serverIP, DrawingData.DrawingData drawingData)
        {
            this.ServerIP = serverIP;
            this.DrawingData = drawingData;
        }
    }
}
