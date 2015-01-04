using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.DrawingData.Deserializers
{
    public interface IDrawingDataDeserializer
    {
        DrawingData Deserialize(byte[] stream);
    }
}
