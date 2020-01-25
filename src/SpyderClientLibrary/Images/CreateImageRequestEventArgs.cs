using System;

namespace Spyder.Client.Images
{
    public class CreateImageRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Target size of generated image being requested
        /// </summary>
        public ImageSize ImageSize { get; set; }

        /// <summary>
        /// Boolean flag that must be set by a handling party to indicate that the request is being processed
        /// </summary>
        public bool Handled { get; set; }
    }
}
