using Spyder.Client.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Images
{
    public class ImageStreamSetRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// Identifier used to identify this request
        /// </summary>
        public QFTThumbnailIdentifier Identifier { get; set; }

        /// <summary>
        /// Function pointer returning a handler to set a native image stream to disk
        /// </summary>
        public Func<QFTThumbnailIdentifier, Stream, FileProgressDelegate, Task<bool>> SetNativeImageStream { get; set; }
    }
}
