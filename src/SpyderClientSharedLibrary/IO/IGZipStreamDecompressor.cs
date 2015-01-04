using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.IO
{
    /// <summary>
    /// Interface used to identify a decompression algorithm for a GZip compressed stream
    /// </summary>
    public interface IGZipStreamDecompressor : IStreamDecompressor
    {
    }
}
