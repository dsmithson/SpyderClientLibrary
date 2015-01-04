using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.IO
{
    /// <summary>
    /// Decompresses a provided zip stream
    /// </summary>
    public interface IStreamDecompressor
    {
        byte[] Decompress(byte[] zipCompressedData, int offset, int count, int uncompressedDataLength);
    }
}
