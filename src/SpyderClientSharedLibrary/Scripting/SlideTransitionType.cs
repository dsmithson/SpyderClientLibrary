using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Scripting
{
    public enum SlideTransitionType
    {
        /// <summary>
        /// No effect is applied
        /// </summary>
        None,

        /// <summary>
        /// Places source immediately on screen
        /// </summary>
        Cut,

        /// <summary>
        /// Simple mix onto output
        /// </summary>
        Fade,

        /// <summary>
        /// Grows from center while mixing on screen
        /// </summary>
        Expand,

        /// <summary>
        /// Starts zoomed in on image and zooms out while fading onto the output
        /// </summary>
        FadedZoom,

        /// <summary>
        /// Rises up from the below the target position
        /// </summary>
        Ascend,

        /// <summary>
        /// Starts stretched horizontally and compresses back into normal shape
        /// </summary>
        Compress,

        /// <summary>
        /// Lowers from above the target position
        /// </summary>
        Descend,

        /// <summary>
        /// Motion effect that slows down as layer moves into position on output
        /// </summary>
        EaseIn,

        /// <summary>
        /// Rises up and overshoots target position slightly, then rubber bands into place
        /// </summary>
        RiseUp,

        /// <summary>
        /// Starts compressed horizontally and decompresses into target keyframe
        /// </summary>
        Stretch,

        /// <summary>
        /// Begins zoomed into input, and zooms out while mixing onto the screen
        /// </summary>
        Zoom,

        /// <summary>
        /// Non-linear effect following a circular pattern while moving into position
        /// </summary>
        CurveUp,

        /// <summary>
        /// Twists layer out of aspect ratio as it is positioned onto the screen
        /// </summary>
        Glide,

        /// <summary>
        /// Zooms in on image during first part of transition and distorts AR before moving to target position
        /// </summary>
        Magnify,

        /// <summary>
        /// Follows circular pattern while moving into position
        /// </summary>
        SpiralIn,

        /// <summary>
        /// Removes layer from output completely
        /// </summary>
        Remove
    }
}
