using Knightware.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.IO
{
    public class GZipStreamDecompressor : IGZipStreamDecompressor
    {
        public byte[] Decompress(byte[] compressedData, int offset, int count, int uncompressedDataLength)
        {
            using (MemoryStream compressedStream = new MemoryStream(compressedData, offset, count, false))
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
