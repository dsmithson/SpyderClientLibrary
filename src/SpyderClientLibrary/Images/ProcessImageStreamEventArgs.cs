using System;
using System.IO;

namespace Spyder.Client.Images
{
    public class ProcessImageStreamEventArgs<K, T> : EventArgs
    {
        /// <summary>
        /// Key used to identify the resource associated with the stream being processed
        /// </summary>
        public K Identifier { get; private set; }

        /// <summary>
        /// Source image stream, which needs to be turned into the type of T
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Resulting T item created by the provided stream.
        /// </summary>
        public T Result { get; set; }

        public ProcessImageStreamEventArgs(K identifier, Stream stream)
        {
            this.Identifier = identifier;
            this.Stream = stream;
        }
    }
}
