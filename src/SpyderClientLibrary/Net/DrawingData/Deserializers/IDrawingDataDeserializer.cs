namespace Spyder.Client.Net.DrawingData.Deserializers
{
    public interface IDrawingDataDeserializer
    {
        DrawingData Deserialize(byte[] stream);
    }
}
