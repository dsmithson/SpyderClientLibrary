using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knightware.Primitives;

namespace Spyder.Client.Common
{
    public static class LayerHelpers
    {
        public static Rectangle GetAbsoluteRectangle(KeyFrame layerKeyFrame, float LayerAspectRatio, PixelSpace parentPixelSpace)
        {
            if (layerKeyFrame == null || parentPixelSpace == null)
                return Rectangle.Empty;

            return GetAbsoluteRectangle(layerKeyFrame, LayerAspectRatio, parentPixelSpace.Rect, parentPixelSpace.Scale);
        }

        public static Rectangle GetAbsoluteRectangle(KeyFrame layerKeyFrame, float layerAspectRatio, Rectangle parentPixelSpaceRect, float parentPixelSpaceScale)
        {
            if (layerKeyFrame == null || parentPixelSpaceRect.IsEmpty)
            {
                return new Rectangle(0, 0, 0, 0);
            }
            else
            {
                // pixel space half width
                float psHW = parentPixelSpaceRect.Width / 2f;
                // ps horiz center
                float psHC = parentPixelSpaceRect.X + psHW;
                // ps half height
                float psHH = parentPixelSpaceRect.Height / 2f;
                // ps vert center
                float psVC = parentPixelSpaceRect.Y + psHH;
                // window horizontal size before crop
                float w;
                if (parentPixelSpaceScale == 1.0F)
                {
                    w = layerKeyFrame.Width;
                }
                else
                {
                    w = layerKeyFrame.Width * parentPixelSpaceScale;
                }
                // vertical size before crop
                float v = w / (layerAspectRatio + layerKeyFrame.AspectRatioOffset);

                // crops in pixels
                float topCrop = v * layerKeyFrame.TopCrop;
                float leftCrop = w * layerKeyFrame.LeftCrop;
                float botCrop = v * layerKeyFrame.BottomCrop;
                float rightCrop = w * layerKeyFrame.RightCrop;

                // window horizontal center before crop
                float hc = psHC + (layerKeyFrame.HPosition * psHW);
                // window vertical center before crop
                float vc = psVC + (layerKeyFrame.VPosition * psHH);

                // actual size after crop
                float hSize = w - leftCrop - rightCrop;
                if (hSize < 0)
                    hSize = 0;
                float vSize = v - topCrop - botCrop;
                if (vSize < 0)
                    vSize = 0;

                // rectange return
                int x, y;

                // Window Center means center is the center of the output window, with crop values
                // applied.
                if (layerKeyFrame.CropAnchor == CropAnchorTypes.WindowCenter)
                {
                    x = (int)Math.Round(hc - (hSize / 2f));
                    y = (int)Math.Round(vc - (vSize / 2f));
                    return new Rectangle(x, y, (int)Math.Round(hSize), (int)Math.Round(vSize));
                }
                // InputCenter means the center of the input is the center, without crop values 
                // so we add left and top crop back in
                else
                {
                    x = (int)Math.Round(hc - (w / 2f) + leftCrop);
                    y = (int)Math.Round(vc - (v / 2f) + topCrop);
                    return new Rectangle(x, y, (int)Math.Round(hSize), (int)Math.Round(vSize));
                }
            }
        }

        public static Rectangle GetAOIRectangle(KeyFrame kf, InputConfig ic)
        {
            if (kf == null || ic == null)
                return Rectangle.Empty;

            return GetAOIRectangle(kf, ic.HActive, ic.VActive, ic.CropOffsetLeft, ic.CropOffsetTop, ic.CropOffsetRight, ic.CropOffsetBottom);
        }

        public static Rectangle GetAOIRectangle(KeyFrame kf, int hActive, int vActive, int inputLeftCrop, int inputTopCrop, int inputRightCrop, int inputBottomCrop)
        {
            if (kf == null)
                return Rectangle.Empty;
            
            int l, t, b, r;
            float width = ((float)1 / kf.Zoom) * hActive;
            float height = ((float)1 / kf.Zoom) * vActive;
            l = (int)Math.Round(((hActive - width) / 2) + kf.PanH);
            if (l < 0)
                l = 0;

            r = (int)Math.Round(l + width);
            if (r > hActive)
            {
                r = hActive;
                l = hActive - (int)width;
            }

            t = (int)Math.Round(((vActive - height) / 2) + kf.PanV);
            if (t < 0)
                t = 0;

            b = (int)Math.Round(t + height);
            if (b > vActive)
            {
                b = vActive;
                t = vActive - (int)height;
            }

            //Apply crop adjustments
            l += (int)Math.Round(kf.LeftCrop * width) + inputLeftCrop;
            r -= (int)Math.Round(kf.RightCrop * width) + inputRightCrop;
            t += (int)Math.Round(kf.TopCrop * height) + inputTopCrop;
            b -= (int)Math.Round(kf.BottomCrop * height) + inputBottomCrop;
            if (r < l)
                r = l;
            if (b < t)
                b = t;

            return new Rectangle(l, t, r - l, b - t);
        }

        public static Rectangle GetCloneRectangle(KeyFrame kf, Rectangle parentPixelSpaceRect, Rectangle absolute)
        {
            if (kf == null || parentPixelSpaceRect.IsEmpty)
            {
                return Rectangle.Empty;
            }

            // pixel space half width
            int psHW = parentPixelSpaceRect.Width / 2;
            // ps horiz center
            int psHC = parentPixelSpaceRect.X + psHW;

            Rectangle result = absolute;
            if (kf.CloneMode == CloneMode.Mirror)
            {
                //result.X = psHC + (int)((kf.HPos * -1f) * (float)psHW) - (absolute.Width / 2);
                result.X = psHC + (psHC - absolute.X - absolute.Width);
            }
            else if (kf.CloneMode == CloneMode.Offset)
            {
                //result.X = psHC + (int)((kf.HPos + kf.CloneOffset) * (float)psHW) - (absolute.Width / 2);
                result.X = absolute.X + (int)((float)psHW * kf.CloneOffset);
            }

            return result;
        }
    }
}
