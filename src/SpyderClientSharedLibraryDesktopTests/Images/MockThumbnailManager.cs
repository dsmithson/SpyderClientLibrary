using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder.Client.Images
{
    public delegate void MockImageCreateHandler(MockImageCreateEventArgs e);

    public class MockThumbnailManager : ThumbnailManagerBase<int, string, MockThumbnailImage>
    {
        public event MockImageCreateHandler CreateImageRequested;
        protected void OnCreateImageRequested(MockImageCreateEventArgs e)
        {
            if (CreateImageRequested != null)
                CreateImageRequested(e);
        }

        protected override Task<string> GenerateImageAsync(int identifier, ImageSize targetSize)
        {
            var args = new MockImageCreateEventArgs(targetSize, identifier);
            OnCreateImageRequested(args);
            return Task.FromResult(args.Result);
        }
    }
}
