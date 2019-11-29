namespace Spyder.Client.Common
{
    public enum BorderStretchMode
    {
        /// <summary>
        /// Bitmap border is pulled out of aspect ratio to fill the aspect ratio of the fill image
        /// </summary>
        Fill,

        /// <summary>
        /// Bitmap border is sized to maintain aspect ratio within the bounding box of the fill image
        /// </summary>
        Uniform,

        /// <summary>
        /// Border uses a custom, user specified aspect ratio
        /// </summary>
        Custom
    }
}
