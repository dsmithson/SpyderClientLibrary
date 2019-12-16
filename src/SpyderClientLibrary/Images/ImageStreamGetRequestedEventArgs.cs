using Knightware.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Spyder.Client.Images
{
    public class ImageStreamGetRequestedEventArgs : EventArgs
    {
        private readonly List<RequestDeferral> deferrals = new List<RequestDeferral>();

        /// <summary>
        /// Identifier used to identify this request
        /// </summary>
        public QFTThumbnailIdentifier Identifier { get; set; }

        /// <summary>
        /// Function pointer returning an image stream for the provided identifier
        /// </summary>
        public Func<QFTThumbnailIdentifier, Task<Stream>> GetNativeImageStream { get; set; }

        /// <summary>
        /// Allows an event handler to request an event processing deferral to allow for async operations to be performed
        /// </summary>
        /// <returns>RequestDeferral pertaining to the request.  The Complete() method must be called when the deferral is finished to prevent application hangs.</returns>
        public RequestDeferral GetDeferral()
        {
            var deferral = new RequestDeferral();
            this.deferrals.Add(deferral);
            return deferral;
        }

        public Task WaitForAllPendingDeferralsAsync()
        {
            return RequestDeferral.WaitForAllCompletedAsync(deferrals);
        }
    }
}
