using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Images
{
    public class MockImageCreateEventArgs : EventArgs
    {
        public ImageSize Size { get; private set; }
        public int Identifier { get; private set; }

        /// <summary>
        /// Resulting string that should be passed back to the source image being updated.  Allows unit tests to inject their own result for test validation.
        /// </summary>
        public string Result { get; set; }

        public MockImageCreateEventArgs(ImageSize size, int identifier)
        {
            this.Size = size;
            this.Identifier = identifier;
        }
    }
}
