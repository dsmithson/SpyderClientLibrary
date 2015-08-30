using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;
using Knightware.Primitives;

namespace Spyder.Client.Models.StackupProviders
{
    public class PreviewProgramStackup : IStackupProvider
    {
        private double displayScale;
        private Point displayTopLeftOffset;
        private Dictionary<int, StackupMap> stackupMaps = new Dictionary<int, StackupMap>();

        public event EventHandler StackupInvalidated;
        protected void OnStackupInvalidated(EventArgs e)
        {
            if (StackupInvalidated != null)
                StackupInvalidated(this, e);
        }

        /// <summary>
        /// Determines if preview pixelspaces should be included in the stackup
        /// </summary>
        public bool ShowPreview { get; set; }

        public Rectangle GenerateOffsetRect(Rectangle sourceRect, int pixelSpaceID)
        {
            if (stackupMaps.ContainsKey(pixelSpaceID))
            {
                StackupMap map = stackupMaps[pixelSpaceID];

                //Don't show preview PixelSpaces?
                if (!ShowPreview && map.Scale != 1)
                    return Rectangle.Empty;

                //Apply scale to the offset from the relocated position
                int offsetX = (int)Math.Round((sourceRect.X - map.OriginalPosition.X) * map.Scale);
                int offsetY = (int)Math.Round((sourceRect.Y - map.OriginalPosition.Y) * map.Scale);

                return new Rectangle()
                {
                    X = (int)Math.Round((map.NewPosition.X + offsetX) * displayScale) + displayTopLeftOffset.X,
                    Y = (int)Math.Round((map.NewPosition.Y + offsetY) * displayScale) + displayTopLeftOffset.Y,
                    Width = (int)Math.Round(sourceRect.Width * map.Scale * displayScale),
                    Height = (int)Math.Round(sourceRect.Height * map.Scale * displayScale)
                };
            }

            return Rectangle.Empty;
        }

        public void UpdateStackupMap(IEnumerable<PixelSpace> pixelSpaces, Size displaySize)
        {
            stackupMaps.Clear();
            if (pixelSpaces == null)
                return;

            int minProgramLeft = int.MaxValue;
            int minProgramTop = int.MaxValue;
            int maxProgramBottom = 0;
            int maxProgramRight = 0;
            int minPreviewLeft = int.MaxValue;
            foreach (PixelSpace pixelSpace in pixelSpaces)
            {
                var r = pixelSpace.Rect;
                if (pixelSpace.Scale == 1)
                {
                    if (r.Left < minProgramLeft)
                        minProgramLeft = r.Left;

                    if (r.Top < minProgramTop)
                        minProgramTop = r.Top;

                    if (r.Right > maxProgramRight)
                        maxProgramRight = r.Right;

                    if (r.Bottom > maxProgramBottom)
                        maxProgramBottom = r.Bottom;
                }
                else if (!ShowPreview)
                {
                    //Exclude this pixelspace
                    continue;
                }
                else
                {
                    if (r.Left < minPreviewLeft)
                        minPreviewLeft = r.Left;
                }

                stackupMaps.Add(pixelSpace.ID, new StackupMap()
                {
                    PixelSpaceID = pixelSpace.ID,
                    OriginalPosition = new Point(pixelSpace.Rect.X, pixelSpace.Rect.Y),
                    OriginalSize = new Size(pixelSpace.Rect.Width, pixelSpace.Rect.Height),
                    Scale = 1 / pixelSpace.Scale
                });
            }

            //Do one more update pass to set preview under program.  I'm going to make an assumption that all previews are going to 
            //have the same scale, which may prove to be wrong someday...
            foreach (StackupMap map in stackupMaps.Values)
            {
                var r = map.OriginalPosition;
                if (map.Scale == 1)
                {
                    map.NewPosition = new Point(r.X - minProgramLeft, r.Y - minProgramTop);
                }
                else
                {
                    map.NewPosition = new Point()
                    {
                        X = (int)Math.Round((r.X - minPreviewLeft) * map.Scale), 
                        Y = (int)Math.Round(maxProgramBottom + 50 + (r.Y * map.Scale)),
                    };
                }
            }

            //Measure our scale based on our display bounds
            if (displaySize.Width > 0 && displaySize.Height > 0 && stackupMaps.Count > 0)
            {
                double nativeWidth = stackupMaps.Values.Max(item => item.NewPosition.X + (item.OriginalSize.Width * item.Scale));
                double nativeHeight = stackupMaps.Values.Max(item => item.NewPosition.Y + (item.OriginalSize.Height * item.Scale));
                displayScale = displaySize.Width / nativeWidth;
                if (nativeHeight * displayScale > displaySize.Height)
                    displayScale = displaySize.Height / nativeHeight;

                displayTopLeftOffset = new Point()
                {
                    X = (int)Math.Round((displaySize.Width - (nativeWidth * displayScale)) / 2),
                    Y = (int)Math.Round((displaySize.Height - (nativeHeight * displayScale)) / 2)
                };
            }
        }

        private class StackupMap
        {
            public int PixelSpaceID { get; set; }
            
            /// <summary>
            /// Original position associated with the PixelSpaceID
            /// </summary>
            public Point OriginalPosition { get; set; }

            /// <summary>
            /// Original size associated with the PixelSpaceID
            /// </summary>
            public Size OriginalSize { get; set; }
                        
            /// <summary>
            /// New position of the pixelspace in the stackup
            /// </summary>
            public Point NewPosition { get; set; }

            /// <summary>
            /// Scale ratio to be applied to a rectangle/point being translated into the stackup space
            /// </summary>
            public double Scale { get; set; }
        }
    }
}
