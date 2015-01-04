using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;
using Spyder.Client.Primitives;

namespace Spyder.Client.ViewModels.Drawing
{
    public enum RepositionMode { None, FrameGroups, AllPixelSpaces }

    public class ViewStackProvider
    {
        private IEnumerable<PixelSpace> pixelSpaces;
        private Dictionary<int, PixelSpaceMap> repositionMap = new Dictionary<int, PixelSpaceMap>();

        /// <summary>
        /// Event raised when a viewstack reposition operation is needed
        /// </summary>
        public event EventHandler RepositionRequested;
        protected void OnRepositionRequested(EventArgs e)
        {
            if (RepositionRequested != null)
                RepositionRequested(this, e);
        }

        private RepositionMode mode = RepositionMode.None;
        public RepositionMode Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    UpdatePixelSpaceMap();
                }
            }
        }

        public void SetPixelSpaces(IEnumerable<PixelSpace> pixelSpaces)
        {
            this.pixelSpaces = pixelSpaces;
            UpdatePixelSpaceMap();
        }

        /// <summary>
        /// Returns the new position for a specified PixelSpaceID, or null if the pixelspace should not be visible
        /// </summary>
        public Point? GetPixelSpacePosition(int pixelSpaceID)
        {
            if (repositionMap == null || !repositionMap.ContainsKey(pixelSpaceID))
                return null;

            return repositionMap[pixelSpaceID].Position;
        }

        /// <summary>
        /// Gets the offset to be applied to the PixelSpace location, in pixels, or null if the pixelspace should not be visible
        /// </summary>
        public Point? GetPixelSpacePositionOffset(int pixelSpaceID)
        {
            if (repositionMap == null || !repositionMap.ContainsKey(pixelSpaceID))
                return null;

            return repositionMap[pixelSpaceID].Offset;
        }

        private void UpdatePixelSpaceMap()
        {
            if (pixelSpaces == null)
            {
                repositionMap.Clear();
                return;
            }

            //TODO:  Use modes to intelligently update the map, and make it thread-safe and processed on a background thread possibly
            repositionMap.Clear();
            foreach (var pixelSpace in pixelSpaces)
            {
                repositionMap.Add(pixelSpace.ID, new PixelSpaceMap()
                {
                    Offset = new Point(0, 0),
                    Position = new Point(pixelSpace.Rect.X, pixelSpace.Rect.Y)
                });
            }

            //Raise notification event
            //TODO:  Make this smart and only fire it if something actually changed
            OnRepositionRequested(EventArgs.Empty);
        }

        private class PixelSpaceMap
        {
            public Point Position { get; set; }
            public Point Offset { get; set; }
        }
    }
}
