using Knightware.Primitives;
using Spyder.Client.Common;

namespace Spyder.Client.Net
{
    public class LayerKeyFrameInfo
    {
        public double HPosition { get; set; }
        public double VPosition { get; set; }
        public Rectangle Rect { get; set; }

        public int BorderThickness { get; set; }
        public Color BorderColor { get; set; }
        public int BorderHBezel { get; set; }
        public int BorderVBezel { get; set; }
        public int BorderInsideSoftness { get; set; }
        public int BorderOutsideSoftness { get; set; }

        public bool OutsideSoftLeft { get; set; }
        public bool OutsideSoftRight { get; set; }
        public bool OutsideSoftTop { get; set; }
        public bool OutsideSoftBottom { get; set; }

        public int ShadowHOffset { get; set; }
        public int ShadowVOffset { get; set; }
        public int ShadowHSize { get; set; }
        public int ShadowSoftness { get; set; }
        public int ShadowTransparency { get; set; }

        public CloneMode Clone { get; set; }
        public double CloneOffset { get; set; }

        public double CropLeft { get; set; }
        public double CropRight { get; set; }
        public double CropTop { get; set; }
        public double CropBottom { get; set; }
        public CropAnchorTypes CropAnchor { get; set; }

        public double AspectRatioOffset { get; set; }
        public double Zoom { get; set; }
        public int PanHorizontal { get; set; }
        public int PanVertical { get; set; }

        public int PixelSpaceID { get; set; }

        public byte Transparency { get; set; }

        public int LayerID { get; set; }

        public LayerKeyFrameInfo()
        {

        }

        public LayerKeyFrameInfo(DrawingData.DrawingKeyFrame dkf)
        {
            CopyFrom(dkf);
        }

        public void CopyFrom(DrawingData.DrawingKeyFrame dkf)
        {
            if (dkf == null)
                return;

            var kf = dkf.KeyFrame;
            if (kf == null)
                return;

            this.LayerID = dkf.LayerID;
            this.AspectRatioOffset = kf.AspectRatioOffset;
            this.BorderColor = kf.BorderColor;
            this.BorderHBezel = kf.BorderLumaOffsetLeft;
            this.BorderVBezel = kf.BorderLumaOffsetTop;
            this.BorderInsideSoftness = kf.BorderInsideSoftness;
            this.BorderOutsideSoftness = kf.BorderOutsideSoftness;
            this.BorderThickness = kf.BorderThickness;
            this.Clone = kf.CloneMode;
            this.CloneOffset = kf.CloneOffset;
            this.CropAnchor = kf.CropAnchor;
            this.CropBottom = kf.BottomCrop;
            this.CropLeft = kf.LeftCrop;
            this.CropRight = kf.RightCrop;
            this.CropTop = kf.TopCrop;
            this.HPosition = kf.HPosition;
            this.OutsideSoftBottom = kf.BorderOutsideSoftBottom;
            this.OutsideSoftLeft = kf.BorderOutsideSoftLeft;
            this.OutsideSoftRight = kf.BorderOutsideSoftRight;
            this.OutsideSoftTop = kf.BorderOutsideSoftTop;
            this.PanHorizontal = kf.PanH;
            this.PanVertical = kf.PanV;
            this.PixelSpaceID = dkf.PixelSpaceID;
            this.Rect = dkf.LayerRect;
            this.ShadowHOffset = kf.ShadowHOffset;
            this.ShadowHSize = kf.ShadowHSize;
            this.ShadowSoftness = kf.ShadowSoftness;
            this.ShadowTransparency = kf.ShadowTransparency;
            this.ShadowVOffset = kf.ShadowVOffset;
            this.Transparency = dkf.Transparency;
            this.VPosition = kf.VPosition;
            this.Zoom = kf.Zoom;
        }
    }
}
