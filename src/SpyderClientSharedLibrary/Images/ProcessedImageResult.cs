using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knightware.Primitives;

namespace Spyder.Client.Images
{
    public class ProcessedImageResult
    {
        public Stream ScaledStream { get; set; }
        public Size NativeSize { get; set; }
        public Size ScaledSize { get; set; }
    }
}
