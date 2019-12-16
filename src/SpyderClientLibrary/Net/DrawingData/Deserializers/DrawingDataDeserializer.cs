using Knightware.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Spyder.Client.Net.DrawingData.Deserializers
{
    /// <summary>
    /// Manages a collection of network packets and deserializes them to drawing data objects when available
    /// </summary>
    public class DrawingDataDeserializer
    {
        private const int HEADER_SIZE = 7;

        private int rxSequence = -1;
        private int rxPacketCount = -1;
        private readonly SortedDictionary<int, byte[]> rxCache = new SortedDictionary<int, byte[]>();

        public string ServerIP { get; private set; }
        public string ServerVersion { get; private set; }

        public DrawingDataDeserializer(string serverIP, string serverVersion)
        {
            this.ServerIP = serverIP;
            this.ServerVersion = serverVersion;
        }

        /// <summary>
        /// Event raised when a new DrawingData object has been deserialized
        /// </summary>
        public event EventHandler<DrawingData> DrawingDataDeserialized;
        protected void OnDrawingDataDeserialized(DrawingData data)
        {
            if (DrawingDataDeserialized != null)
            {
                DrawingDataDeserialized(this, data);
            }
        }

        public void Read(byte[] Stream, int offset)
        {
            int readIndex = offset;

            if (Stream == null || Stream.Length < HEADER_SIZE)
                return;

            byte version = Stream[readIndex++];
            byte compression = Stream[readIndex++];
            byte sequence = Stream[readIndex++];
            byte packetID = Stream[readIndex++];
            byte totalPackets = Stream[readIndex++];
            int streamLength = Stream[readIndex++];
            streamLength |= (Stream[readIndex++] << 8);
            bool compressed = (compression == (byte)'C');

            var deserializer = GetDeserializer(version, ServerVersion);
            if (deserializer == null)
            {
                //Ignore drawing data that we don't have a deserializer for
                return;
            }

            if (streamLength > Stream.Length - offset)
            {
                TraceQueue.Trace(this, TracingLevel.Information, "DrawingData: Invalid packet size specified in header: {0}", streamLength);
                return;
            }

            //New Sequence?
            if (sequence != rxSequence)
            {
                rxCache.Clear();
                rxSequence = sequence;
                rxPacketCount = totalPackets;
            }

            //Validate packet total size
            if (totalPackets != this.rxPacketCount)
            {
                TraceQueue.Trace(this, TracingLevel.Information, "DrawingData: Invalid packet count in current sequence");
                return;
            }

            //Computers with multiple network cards may give us the same drawing data packet more than once
            if (rxCache.ContainsKey(packetID))
            {
                return;
            }

            //Copy data from packet
            byte[] packetData = new byte[streamLength];
            Array.Copy(Stream, readIndex, packetData, 0, streamLength);
            rxCache.Add(packetID, packetData);

            //Last Packet
            if (rxCache.Count == totalPackets)
            {
                //Create full packet
                byte[] fullRxPacket = new byte[rxCache.Values.Sum(array => (array == null ? 0 : array.Length))];
                int index = 0;
                for (int i = 0; i < totalPackets; i++)
                {
                    byte[] current = rxCache[i];
                    if (current == null)
                        return;

                    int currentLength = current.Length;
                    Array.ConstrainedCopy(current, 0, fullRxPacket, index, currentLength);
                    index += currentLength;
                }

                //Decompress packet if needed
                if (compressed)
                {
                    fullRxPacket = Decompress(fullRxPacket, 0, fullRxPacket.Length);
                    if (fullRxPacket == null)
                    {
                        TraceQueue.Trace(this, TracingLevel.Warning, "Failed to decompress drawing data");
                        return;
                    }
                }

                //Deserialize data
                DrawingData drawingData;
                drawingData = deserializer.Deserialize(fullRxPacket);
                if (drawingData == null)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Failed to deserialize drawing data");
                    return;
                }

                //Raise drawing data update event
                OnDrawingDataDeserialized(drawingData);

                rxCache.Clear();
            }
        }

        private IDrawingDataDeserializer GetDeserializer(int drawingDataVersion, string serverVersion)
        {
            switch (drawingDataVersion)
            {
                case 8: return new DrawingDataDeserializer_Version8();
                case 19: return new DrawingDataDeserializer_Version19();
                case 20: return new DrawingDataDeserializer_Version20(serverVersion);
                case 50: return new DrawingDataDeserializer_Version50(serverVersion);
                case 51: return new DrawingDataDeserializer_Version51(serverVersion);
                case 52: return new DrawingDataDeserializer_Version52(serverVersion);
                case 53: return new DrawingDataDeserializer_Version53(serverVersion);
                default: return null;
            }
        }

        private byte[] Decompress(byte[] compressedData, int offset, int count)
        {
            if (compressedData == null)
            {
                return null;
            }

            try
            {
                //Read the uncompressed size of the stream from the first 4 bytes of the source array
                int uncompressedSize = 0;
                uncompressedSize |= (compressedData[offset]);
                uncompressedSize |= (compressedData[offset + 1] << 8);
                uncompressedSize |= (compressedData[offset + 2] << 16);
                uncompressedSize |= (compressedData[offset + 3] << 24);

                return Decompress(compressedData, offset + 4, count - 4, uncompressedSize);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while decompressing DrawingData: {1}", ex.GetType().Name, ex.Message);
                return null;
            }
        }

        public byte[] Decompress(byte[] zipCompressedData, int offset, int count, int uncompressedDataLength)
        {
            using (MemoryStream compressedStream = new MemoryStream(zipCompressedData, offset, count, false))
            {
                using (var decompressor = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    byte[] decompressedBytes = new byte[uncompressedDataLength];
                    int read = decompressor.Read(decompressedBytes, 0, uncompressedDataLength);
                    return (read == uncompressedDataLength ? decompressedBytes : null);
                }
            }
        }
    }
}
