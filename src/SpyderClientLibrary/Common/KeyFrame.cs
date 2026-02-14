using Knightware.Primitives;
using System;
using Spyder.Client;

namespace Spyder.Client.Common
{
    public class KeyFrame : PropertyChangedBase, IEquatable<KeyFrame>
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

        /// <summary>
        /// Spyder-S supports up to 4 clones
        /// </summary>
        public float[] CloneOffsets
        {
            get { return cloneOffsets; }
            set
            {
                if(!cloneOffsets.SequenceEqualSafe(value))
                {
                    cloneOffsets = value;
                    OnPropertyChanged(nameof(CloneOffsets));
                    OnPropertyChanged(nameof(CloneOffset));
                }
            }
        }
        private float[] cloneOffsets;

        /// <summary>
        /// Gets or sets the offset value for the first clone in the collection - all Spyder types before Spyder-S support only one clone
        /// </summary>
        public float CloneOffset
        {
            get 
            {
                if (cloneOffsets == null || cloneOffsets.Length < 1)
                    return 0f;

                return cloneOffsets[0];
            }
            set
            {
                if (value == 0)
                    CloneOffsets = null;
                else
                    CloneOffsets = new float[] { value };
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

        private byte transparency;
        public byte Transparency
        {
            get { return transparency; }
            set
            {
                if (transparency != value)
                {
                    transparency = value;
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

        private BorderStretchMode borderShapeStretch = BorderStretchMode.Fill;
        public BorderStretchMode BorderShapeStretch
        {
            get { return borderShapeStretch; }
            set
            {
                if (borderShapeStretch != value)
                {
                    borderShapeStretch = value;
                    OnPropertyChanged();
                }
            }
        }

        private float borderShapeStretchAspectRatio = 1.777f;
        public float BorderShapeStretchAspectRatio
        {
            get { return borderShapeStretchAspectRatio; }
            set
            {
                if (borderShapeStretchAspectRatio != value)
                {
                    borderShapeStretchAspectRatio = value;
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
            Transparency = copyFrom.Transparency;
            UseDefaultMotionValues = copyFrom.UseDefaultMotionValues;

            BorderFillSource = copyFrom.BorderFillSource;
            BorderTileMode = copyFrom.BorderTileMode;
            BorderTextureType = copyFrom.BorderTextureType;
            BorderTextureFile = copyFrom.BorderTextureFile;

            BorderShapeSource = copyFrom.BorderShapeSource;
            BorderShape = copyFrom.BorderShape;
            BorderShapeFile = copyFrom.BorderShapeFile;
            BorderShapeStretch = copyFrom.BorderShapeStretch;
            BorderShapeStretchAspectRatio = copyFrom.BorderShapeStretchAspectRatio;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(KeyFrame compare)
        {
            if (compare == null)
                return false;

            if (this.Tension != compare.Tension) return false;
            if (this.Bias != compare.Bias) return false;
            if (this.Continuity != compare.Continuity) return false;
            if (this.EaseIn != compare.EaseIn) return false;
            if (this.EaseOut != compare.EaseOut) return false;
            if (this.Duration != compare.Duration) return false;
            if (this.HPosition != compare.HPosition) return false;
            if (this.VPosition != compare.VPosition) return false;
            if (this.Width != compare.Width) return false;
            if (this.BorderColor != compare.BorderColor) return false;
            if (this.ShadowColor != compare.ShadowColor) return false;
            if (this.BorderInsideSoftness != compare.BorderInsideSoftness) return false;
            if (this.BorderOutsideSoftness != compare.BorderOutsideSoftness) return false;
            if (this.BorderLumaOffsetTop != compare.BorderLumaOffsetTop) return false;
            if (this.BorderLumaOffsetLeft != compare.BorderLumaOffsetLeft) return false;
            if (this.BorderLumaOffsetRight != compare.BorderLumaOffsetRight) return false;
            if (this.BorderLumaOffsetBottom != compare.BorderLumaOffsetBottom) return false;
            if (this.BorderThickness != compare.BorderThickness) return false;
            if (this.BorderOutsideSoftTop != compare.BorderOutsideSoftTop) return false;
            if (this.BorderOutsideSoftLeft != compare.BorderOutsideSoftLeft) return false;
            if (this.BorderOutsideSoftRight != compare.BorderOutsideSoftRight) return false;
            if (this.BorderOutsideSoftBottom != compare.BorderOutsideSoftBottom) return false;
            if (this.ShadowHOffset != compare.ShadowHOffset) return false;
            if (this.ShadowVOffset != compare.ShadowVOffset) return false;
            if (this.ShadowTransparency != compare.ShadowTransparency) return false;
            if (this.ShadowSoftness != compare.ShadowSoftness) return false;
            if (this.ShadowHSize != compare.ShadowHSize) return false;
            if (this.ShadowVSize != compare.ShadowVSize) return false;
            if (this.CloneMode != compare.CloneMode) return false;
            if (this.CloneOffset != compare.CloneOffset) return false;
            if (this.TopCrop != compare.TopCrop) return false;
            if (this.LeftCrop != compare.LeftCrop) return false;
            if (this.RightCrop != compare.RightCrop) return false;
            if (this.BottomCrop != compare.BottomCrop) return false;
            if (this.CropAnchor != compare.CropAnchor) return false;
            if (this.PanH != compare.PanH) return false;
            if (this.PanV != compare.PanV) return false;
            if (this.Zoom != compare.Zoom) return false;
            if (this.Transparency != compare.Transparency) return false;
            if (this.UseDefaultMotionValues != compare.UseDefaultMotionValues) return false;
            if (this.BorderFillSource != compare.BorderFillSource) return false;
            if (this.BorderTileMode != compare.BorderTileMode) return false;
            if (this.BorderTextureType != compare.BorderTextureType) return false;
            if (this.BorderTextureFile != compare.BorderTextureFile) return false;
            if (this.BorderShapeSource != compare.BorderShapeSource) return false;
            if (this.BorderShape != compare.BorderShape) return false;
            if (this.BorderShapeFile != compare.BorderShapeFile) return false;
            if (this.BorderShapeStretch != compare.BorderShapeStretch) return false;
            if (this.BorderShapeStretchAspectRatio != compare.BorderShapeStretchAspectRatio) return false;
            if (this.AspectRatioOffset != compare.AspectRatioOffset) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            KeyFrame compare = obj as KeyFrame;
            if (compare == null)
                return false;
            else
                return this.Equals(compare);
        }

        public static bool operator ==(KeyFrame kf1, KeyFrame kf2)
        {
            if (kf1 is null || kf2 is null)
                return Object.Equals(kf1, kf2);

            return kf1.Equals(kf2);
        }

        public static bool operator !=(KeyFrame kf1, KeyFrame kf2)
        {
            if (kf1 is null || kf2 is null)
                return !Object.Equals(kf1, kf2);

            return !kf1.Equals(kf2);
        }
    }
}
