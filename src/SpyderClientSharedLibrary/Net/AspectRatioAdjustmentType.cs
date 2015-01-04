using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    public enum AspectRatioAdjustmentType
    {
        /// <summary>
        /// Modify the layer keyframe aspect ratio based on the source content aspect ratio to achieve a desired overall aspect ratio
        /// </summary>
        SetLayerAspectRatio,

        /// <summary>
        /// Directly set the aspect ratio offset in the keyframe
        /// </summary>
        SetKeyFrameAspectRatio,

        /// <summary>
        /// Add/Subtract a specified value from the current layer keyframe aspect ratio offset
        /// </summary>
        OffsetKeyFrameAspectRatio
    }
}
