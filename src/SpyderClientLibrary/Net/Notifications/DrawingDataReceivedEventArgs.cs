using System;

namespace Spyder.Client.Net.Notifications
{
    public class DrawingDataReceivedEventArgs : EventArgs
    {
        public string ServerIP { get; private set; }

        public byte[] RawMessage { get; private set; }

        public DrawingData.DrawingData DrawingData { get; private set; }

        public DrawingDataReceivedEventArgs(string serverIP, DrawingData.DrawingData drawingData, byte[] rawMessage)
        {
            this.ServerIP = serverIP;
            this.DrawingData = drawingData;
            this.RawMessage = rawMessage;
        }
    }
}
