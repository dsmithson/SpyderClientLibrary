using System;

namespace Spyder.Client.Net.Sockets
{
    public class DataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
        public int Length { get; set; }
        public string SenderAddress { get; set; }
    }
}
