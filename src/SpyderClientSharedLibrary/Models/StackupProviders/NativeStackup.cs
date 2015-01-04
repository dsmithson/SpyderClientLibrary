using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;
using Spyder.Client.Primitives;

namespace Spyder.Client.Models.StackupProviders
{
    /// <summary>
    /// For testing purposes - returns a native/pass-thru rectangle environment
    /// </summary>
    public class NativeStackup : IStackupProvider
    {
        public event EventHandler StackupInvalidated;
        protected void OnStackupInvalidated(EventArgs e)
        {
            if (StackupInvalidated != null)
                StackupInvalidated(this, e);
        }

        public Rectangle GenerateOffsetRect(Rectangle sourceRect, int pixelSpaceID)
        {
            return sourceRect;
        }

        public void UpdateStackupMap(IEnumerable<PixelSpace> pixelSpaces, Size displaySize)
        {
            //Nothing to do
        }
    }
}
