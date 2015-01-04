using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Images
{
    public enum ImageSize
    {
        /// <summary>
        /// Native image size (No scaling)
        /// </summary>
        Native = 0,

        /// <summary>
        /// 32x32 pixel image
        /// </summary>
        ExtraSmall = 32,

        /// <summary>
        /// 128x128 pixel image
        /// </summary>
        Small = 128,

        /// <summary>
        /// 512x512 pixel image
        /// </summary>
        Medium = 512,

        /// <summary>
        /// 2048x2048 image
        /// </summary>
        Large = 2048
    }
}
