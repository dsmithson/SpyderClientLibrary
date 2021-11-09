using System;
using System.Collections.Generic;
using System.Text;

namespace Spyder.Client.Net.DrawingData.Deserializers
{
    public delegate void DrawingDataDeserializedEventHandler(object sender, DrawingDataDeserializedEventArgs e);

    public class DrawingDataDeserializedEventArgs : EventArgs
    {
        public DrawingData DrawingData { get; set; }

        public byte[] RawMessage { get; set; }
    }
}
