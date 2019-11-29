using Spyder.Client.Scripting;
using System.Drawing;

namespace Spyder.Client.Common
{
    public enum SlideContentType { Video, Still };

    /// <summary>
    /// Settings for recalling a layer in a PowerPoint style slide transition
    /// </summary>
    public class SlideLayoutEntry
    {
        public string SourceName { get; set; }
        public string FullFilePath { get; set; }
        public SlideContentType ContentType { get; set; }
        public int ZOrder { get; set; }
        public PointF Position { get; set; }
        public Size Size { get; set; }
        public int ShadowTransparency { get; set; }
        public Point ShadowOffset { get; set; }
        public Color BorderColor { get; set; }
        public int BorderThickness { get; set; }
        public SlideTransitionType TransitionType { get; set; }
        public int TransitionDuration { get; set; }
    }
}
