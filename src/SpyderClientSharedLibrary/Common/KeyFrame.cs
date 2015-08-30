using Knightware;
using Knightware.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class KeyFrame : PropertyChangedBase
    {
        private float tension;
        public float Tension
        {
            get { return tension; }
            set
            {
                if (tension != value)
                {
                    tension = value;
                    OnPropertyChanged();
                }
            }
        }

        private float bias;
        public float Bias
        {
            get { return bias; }
            set
            {
                if (bias != value)
                {
                    bias = value;
                    OnPropertyChanged();
                }
            }
        }

        private float continuity;
        public float Continuity
        {
            get { return continuity; }
            set
            {
                if (continuity != value)
                {
                    continuity = value;
                    OnPropertyChanged();
                }
            }
        }

        private float easeIn;
        public float EaseIn
        {
            get { return easeIn; }
            set
            {
                if (easeIn != value)
                {
                    easeIn = value;
                    OnPropertyChanged();
                }
            }
        }

        private float easeOut;
        public float EaseOut
        {
            get { return easeOut; }
            set
            {
                if (easeOut != value)
                {
                    easeOut = value;
                    OnPropertyChanged();
                }
            }
        }

        private int duration;
        public int Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged();
                }
            }
        }

        private float hPosition;
        public float HPosition
        {
            get { return hPosition; }
            set
            {
                if (hPosition != value)
                {
                    hPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        private float vPosition;
        public float VPosition
        {
            get { return vPosition; }
            set
            {
                if (vPosition != value)
                {
                    vPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color borderColor;
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color shadowColor;
        public Color ShadowColor
        {
            get { return shadowColor; }
            set
            {
                if (shadowColor != value)
                {
                    shadowColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private int borderInsideSoftness;
        public int BorderInsideSoftness
        {
            get { return borderInsideSoftness; }
            set
            {
                if (borderInsideSoftness != value)
                {
                    borderInsideSoftness = value;
                    OnPropertyChanged();
                }
            }
        }

        private int borderOutsideSoftness;
        public int BorderOutsideSoftness
        {
            get { return borderOutsideSoftness; }
            set
            {
                if (borderOutsideSoftness != value)
                {
                    borderOutsideSoftness = value;
                    OnPropertyChanged();
                }
            }
        }

        private int borderLumaOffsetTop;
        public int BorderLumaOffsetTop
        {
            get { return borderLumaOffsetTop; }
            set
            {
                if (borderLumaOffsetTop != value)
                {
                    borderLumaOffsetTop = value;
                    OnPropertyChanged();
                }
            }
        }

        private int borderLumaOffsetLeft;
        public int BorderLumaOffsetLeft
        {
            get { return borderLumaOffsetLeft; }
            set
            {
                if (borderLumaOffsetLeft != value)
                {
                    borderLumaOffsetLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        private int borderLumaOffsetRight;
        public int BorderLumaOffsetRight
        {
            get { return borderLumaOffsetRight; }
            set
            {
                if (borderLumaOffsetRight != value)
                {
                    borderLumaOffsetRight = value;
                    OnPropertyChanged();
                }
            }
        }

        private int borderLumaOffsetBottom;
        public int BorderLumaOffsetBottom
        {
            get { return borderLumaOffsetBottom; }
            set
            {
                if (borderLumaOffsetBottom != value)
                {
                    borderLumaOffsetBottom = value;
                    OnPropertyChanged();
                }
            }
        }

        private int borderThickness;
        public int BorderThickness
        {
            get { return borderThickness; }
            set
            {
                if (borderThickness != value)
                {
                    borderThickness = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool borderOutsideSoftTop;
        public bool BorderOutsideSoftTop
        {
            get { return borderOutsideSoftTop; }
            set
            {
                if (borderOutsideSoftTop != value)
                {
                    borderOutsideSoftTop = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool borderOutsideSoftLeft;
        public bool BorderOutsideSoftLeft
        {
            get { return borderOutsideSoftLeft; }
            set
            {
                if (borderOutsideSoftLeft != value)
                {
                    borderOutsideSoftLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool borderOutsideSoftRight;
        public bool BorderOutsideSoftRight
        {
            get { return borderOutsideSoftRight; }
            set
            {
                if (borderOutsideSoftRight != value)
                {
                    borderOutsideSoftRight = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool borderOutsideSoftBottom;
        public bool BorderOutsideSoftBottom
        {
            get { return borderOutsideSoftBottom; }
            set
            {
                if (borderOutsideSoftBottom != value)
                {
                    borderOutsideSoftBottom = value;
                    OnPropertyChanged();
                }
            }
        }

        private int shadowHOffset;
        public int ShadowHOffset
        {
            get { return shadowHOffset; }
            set
            {
                if (shadowHOffset != value)
                {
                    shadowHOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        private int shadowVOffset;
        public int ShadowVOffset
        {
            get { return shadowVOffset; }
            set
            {
                if (shadowVOffset != value)
                {
                    shadowVOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        private int shadowTransparency;
        public int ShadowTransparency
        {
            get { return shadowTransparency; }
            set
            {
                if (shadowTransparency != value)
                {
                    shadowTransparency = value;
                    OnPropertyChanged();
                }
            }
        }

        private int shadowSoftness;
        public int ShadowSoftness
        {
            get { return shadowSoftness; }
            set
            {
                if (shadowSoftness != value)
                {
                    shadowSoftness = value;
                    OnPropertyChanged();
                }
            }
        }

        private int shadowHSize;
        public int ShadowHSize
        {
            get { return shadowHSize; }
            set
            {
                if (shadowHSize != value)
                {
                    shadowHSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private int shadowVSize;
        public int ShadowVSize
        {
            get { return shadowVSize; }
            set
            {
                if (shadowVSize != value)
                {
                    shadowVSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private CloneMode cloneMode;
        public CloneMode CloneMode
        {
            get { return cloneMode; }
            set
            {
                if (cloneMode != value)
                {
                    cloneMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private float cloneOffset;
        public float CloneOffset
        {
            get { return cloneOffset; }
            set
            {
                if (cloneOffset != value)
                {
                    cloneOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        private float topCrop;
        public float TopCrop
        {
            get { return topCrop; }
            set
            {
                if (topCrop != value)
                {
                    topCrop = value;
                    OnPropertyChanged();
                }
            }
        }

        private float leftCrop;
        public float LeftCrop
        {
            get { return leftCrop; }
            set
            {
                if (leftCrop != value)
                {
                    leftCrop = value;
                    OnPropertyChanged();
                }
            }
        }

        private float rightCrop;
        public float RightCrop
        {
            get { return rightCrop; }
            set
            {
                if (rightCrop != value)
                {
                    rightCrop = value;
                    OnPropertyChanged();
                }
            }
        }

        private float bottomCrop;
        public float BottomCrop
        {
            get { return bottomCrop; }
            set
            {
                if (bottomCrop != value)
                {
                    bottomCrop = value;
                    OnPropertyChanged();
                }
            }
        }

        private CropAnchorTypes cropAnchor;
        public CropAnchorTypes CropAnchor
        {
            get { return cropAnchor; }
            set
            {
                if (cropAnchor != value)
                {
                    cropAnchor = value;
                    OnPropertyChanged();
                }
            }
        }

        private int panH;
        public int PanH
        {
            get { return panH; }
            set
            {
                if (panH != value)
                {
                    panH = value;
                    OnPropertyChanged();
                }
            }
        }

        private int panV;
        public int PanV
        {
            get { return panV; }
            set
            {
                if (panV != value)
                {
                    panV = value;
                    OnPropertyChanged();
                }
            }
        }

        private float zoom;
        public float Zoom
        {
            get { return zoom; }
            set
            {
                if (zoom != value)
                {
                    zoom = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool useDefaultMotionValues;
        public bool UseDefaultMotionValues
        {
            get { return useDefaultMotionValues; }
            set
            {
                if (useDefaultMotionValues != value)
                {
                    useDefaultMotionValues = value;
                    OnPropertyChanged();
                }
            }
        }

        private TextureFillSource borderFillSource;
        public TextureFillSource BorderFillSource
        {
            get { return borderFillSource; }
            set
            {
                if (borderFillSource != value)
                {
                    borderFillSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private TextureTileMode borderTileMode;
        public TextureTileMode BorderTileMode
        {
            get { return borderTileMode; }
            set
            {
                if (borderTileMode != value)
                {
                    borderTileMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private TextureType borderTextureType;
        public TextureType BorderTextureType
        {
            get { return borderTextureType; }
            set
            {
                if (borderTextureType != value)
                {
                    borderTextureType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string borderTextureFile;
        public string BorderTextureFile
        {
            get { return borderTextureFile; }
            set
            {
                if (borderTextureFile != value)
                {
                    borderTextureFile = value;
                    OnPropertyChanged();
                }
            }
        }

        private ShapeSource borderShapeSource;
        public ShapeSource BorderShapeSource
        {
            get { return borderShapeSource; }
            set
            {
                if (borderShapeSource != value)
                {
                    borderShapeSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private ShapeType borderShape;
        public ShapeType BorderShape
        {
            get { return borderShape; }
            set
            {
                if (borderShape != value)
                {
                    borderShape = value;
                    OnPropertyChanged();
                }
            }
        }

        private string borderShapeFile;
        public string BorderShapeFile
        {
            get { return borderShapeFile; }
            set
            {
                if (borderShapeFile != value)
                {
                    borderShapeFile = value;
                    OnPropertyChanged();
                }
            }
        }

        private float aspectRatioOffset;
        public float AspectRatioOffset
        {
            get { return aspectRatioOffset; }
            set
            {
                if (aspectRatioOffset != value)
                {
                    aspectRatioOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        public KeyFrame()
        {
        }

        public KeyFrame(KeyFrame copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public virtual void CopyFrom(KeyFrame copyFrom)
        {
            if (copyFrom == null)
                return;

            Tension = copyFrom.Tension;
            Bias = copyFrom.Bias;
            Continuity = copyFrom.Continuity;
            EaseIn = copyFrom.EaseIn;
            EaseOut = copyFrom.EaseOut;
            Duration = copyFrom.Duration;
            HPosition = copyFrom.HPosition;
            VPosition = copyFrom.VPosition;
            Width = copyFrom.Width;
            AspectRatioOffset = copyFrom.AspectRatioOffset;
            BorderColor = new Color(copyFrom.borderColor);
            ShadowColor = new Color(copyFrom.shadowColor);
            BorderInsideSoftness = copyFrom.BorderInsideSoftness;
            BorderOutsideSoftness = copyFrom.BorderOutsideSoftness;
            BorderLumaOffsetTop = copyFrom.BorderLumaOffsetTop;
            BorderLumaOffsetLeft = copyFrom.BorderLumaOffsetLeft;
            BorderLumaOffsetBottom = copyFrom.BorderLumaOffsetBottom;
            BorderLumaOffsetRight = copyFrom.BorderLumaOffsetRight;
            BorderThickness = copyFrom.BorderThickness;
            BorderOutsideSoftBottom = copyFrom.BorderOutsideSoftBottom;
            BorderOutsideSoftLeft = copyFrom.BorderOutsideSoftLeft;
            BorderOutsideSoftRight = copyFrom.BorderOutsideSoftRight;
            BorderOutsideSoftTop = copyFrom.BorderOutsideSoftTop;

            ShadowHOffset = copyFrom.ShadowHOffset;
            ShadowVOffset = copyFrom.ShadowVOffset;
            ShadowTransparency = copyFrom.ShadowTransparency;
            ShadowSoftness = copyFrom.ShadowSoftness;
            ShadowHSize = copyFrom.ShadowHSize;
            ShadowVSize = copyFrom.ShadowVSize;
            CloneMode = copyFrom.CloneMode;
            CloneOffset = copyFrom.CloneOffset;
            CropAnchor = copyFrom.CropAnchor;

            TopCrop = copyFrom.TopCrop;
            BottomCrop = copyFrom.BottomCrop;
            LeftCrop = copyFrom.LeftCrop;
            RightCrop = copyFrom.RightCrop;

            PanH = copyFrom.PanH;
            PanV = copyFrom.PanV;
            Zoom = copyFrom.Zoom;
            UseDefaultMotionValues = copyFrom.UseDefaultMotionValues;

            BorderFillSource = copyFrom.BorderFillSource;
            BorderTileMode = copyFrom.BorderTileMode;
            BorderTextureType = copyFrom.BorderTextureType;
            BorderTextureFile = copyFrom.BorderTextureFile;

            BorderShapeSource = copyFrom.BorderShapeSource;
            BorderShape = copyFrom.BorderShape;
            BorderShapeFile = copyFrom.BorderShapeFile;
        }

        public override bool Equals(object obj)
        {
            var copyFrom = obj as KeyFrame;
            if (copyFrom == null)
                return false;

            if (this.Tension != copyFrom.Tension) return false;
            if (this.Bias != copyFrom.Bias) return false;
            if (this.Continuity != copyFrom.Continuity) return false;
            if (this.EaseIn != copyFrom.EaseIn) return false;
            if (this.EaseOut != copyFrom.EaseOut) return false;
            if (this.Duration != copyFrom.Duration) return false;
            if (this.HPosition != copyFrom.HPosition) return false;
            if (this.VPosition != copyFrom.VPosition) return false;
            if (this.Width != copyFrom.Width) return false;
            if (this.BorderColor != copyFrom.BorderColor) return false;
            if (this.ShadowColor != copyFrom.ShadowColor) return false;
            if (this.BorderInsideSoftness != copyFrom.BorderInsideSoftness) return false;
            if (this.BorderOutsideSoftness != copyFrom.BorderOutsideSoftness) return false;
            if (this.BorderLumaOffsetTop != copyFrom.BorderLumaOffsetTop) return false;
            if (this.BorderLumaOffsetLeft != copyFrom.BorderLumaOffsetLeft) return false;
            if (this.BorderLumaOffsetRight != copyFrom.BorderLumaOffsetRight) return false;
            if (this.BorderLumaOffsetBottom != copyFrom.BorderLumaOffsetBottom) return false;
            if (this.BorderThickness != copyFrom.BorderThickness) return false;
            if (this.BorderOutsideSoftTop != copyFrom.BorderOutsideSoftTop) return false;
            if (this.BorderOutsideSoftLeft != copyFrom.BorderOutsideSoftLeft) return false;
            if (this.BorderOutsideSoftRight != copyFrom.BorderOutsideSoftRight) return false;
            if (this.BorderOutsideSoftBottom != copyFrom.BorderOutsideSoftBottom) return false;
            if (this.ShadowHOffset != copyFrom.ShadowHOffset) return false;
            if (this.ShadowVOffset != copyFrom.ShadowVOffset) return false;
            if (this.ShadowTransparency != copyFrom.ShadowTransparency) return false;
            if (this.ShadowSoftness != copyFrom.ShadowSoftness) return false;
            if (this.ShadowHSize != copyFrom.ShadowHSize) return false;
            if (this.ShadowVSize != copyFrom.ShadowVSize) return false;
            if (this.CloneMode != copyFrom.CloneMode) return false;
            if (this.CloneOffset != copyFrom.CloneOffset) return false;
            if (this.TopCrop != copyFrom.TopCrop) return false;
            if (this.LeftCrop != copyFrom.LeftCrop) return false;
            if (this.RightCrop != copyFrom.RightCrop) return false;
            if (this.BottomCrop != copyFrom.BottomCrop) return false;
            if (this.CropAnchor != copyFrom.CropAnchor) return false;
            if (this.PanH != copyFrom.PanH) return false;
            if (this.PanV != copyFrom.PanV) return false;
            if (this.Zoom != copyFrom.Zoom) return false;
            if (this.UseDefaultMotionValues != copyFrom.UseDefaultMotionValues) return false;
            if (this.BorderFillSource != copyFrom.BorderFillSource) return false;
            if (this.BorderTileMode != copyFrom.BorderTileMode) return false;
            if (this.BorderTextureType != copyFrom.BorderTextureType) return false;
            if (this.BorderTextureFile != copyFrom.BorderTextureFile) return false;
            if (this.BorderShapeSource != copyFrom.BorderShapeSource) return false;
            if (this.BorderShape != copyFrom.BorderShape) return false;
            if (this.BorderShapeFile != copyFrom.BorderShapeFile) return false;
            if (this.AspectRatioOffset != copyFrom.AspectRatioOffset) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
