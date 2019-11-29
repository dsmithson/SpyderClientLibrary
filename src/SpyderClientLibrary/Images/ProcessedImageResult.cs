using Knightware.Primitives;
using System.IO;

namespace Spyder.Client.Images
{
    public class ProcessedImageResult
    {
        public Stream ScaledStream { get; set; }
        public Size NativeSize { get; set; }
        public Size ScaledSize { get; set; }
    }
}
