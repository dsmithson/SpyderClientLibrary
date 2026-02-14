using Knightware.Primitives;
using Spyder.Client.Common;
using System;
using Spyder.Client;

namespace Spyder.Client.Net.DrawingData
{
    public class DrawingKeyFrame : PropertyChangedBase
    {
        private string source;
        public string Source
        {
            get { return source; }
            set
            {
                if (source != value)
                {
                    source = value;
                    OnPropertyChanged();
                }
            }
        }

        private int sourceRouterID;
        public int SourceRouterID
        {
            get { return sourceRouterID; }
            set
            {
                if (sourceRouterID != value)
                {
                    sourceRouterID = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Starting in Spyder-S, global layers are automatically provisioned with static slave layers for each frame group
        /// </summary>
        public bool IsGlobalLayer
        {
            get { return isGlobalLayer; }
            set
            {
                if (isGlobalLayer != value)
                {
                    isGlobalLayer = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool isGlobalLayer;

        /// <summary>
        /// In Spyder-S frames, layers can use mix effects as content
        /// </summary>
        public int MixEffectID
        {
            get { return mixEffectID; }
            set
            {
                if(mixEffectID != value)
                {
                    mixEffectID = value;
                    OnPropertyChanged();
                }
            }
        }
        private int mixEffectID = -1;

        public DrawingMixEffect MixEffect
        {
            get { return mixEffect; }
            set
            {
                if(mixEffect != value)
                {
                    mixEffect = value;
                    OnPropertyChanged();
                }
            }
        }
        private DrawingMixEffect mixEffect;

        private int sourceRouterInput;
        public int SourceRouterInput
        {
            get { return sourceRouterInput; }
            set
            {
                if (sourceRouterInput != value)
                {
                    sourceRouterInput = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool hdcpAuthenticated;
        public bool HdcpAuthenticated
        {
            get { return hdcpAuthenticated; }
            set
            {
                if (hdcpAuthenticated != value)
                {
                    hdcpAuthenticated = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool shadowIsEnabled;
        public bool ShadowIsEnabled
        {
            get { return shadowIsEnabled; }
            set
            {
                if (shadowIsEnabled != value)
                {
                    shadowIsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set
            {
                if (isLocked != value)
                {
                    isLocked = value;
                    OnPropertyChanged();
                }
            }
        }


        private string linearKeySource;
        public string LinearKeySource
        {
            get { return linearKeySource; }
            set
            {
                if (linearKeySource != value)
                {
                    linearKeySource = value;
                    OnPropertyChanged();
                }
            }
        }

        private int linearKeyRouterID;
        public int LinearKeyRouterID
        {
            get { return linearKeyRouterID; }
            set
            {
                if (linearKeyRouterID != value)
                {
                    linearKeyRouterID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int linearKeyRouterInput;
        public int LinearKeyRouterInput
        {
            get { return linearKeyRouterInput; }
            set
            {
                if (linearKeyRouterInput != value)
                {
                    linearKeyRouterInput = value;
                    OnPropertyChanged();
                }
            }
        }


        private KeyFrame keyFrame = new KeyFrame();
        public KeyFrame KeyFrame
        {
            get { return keyFrame; }
            set
            {
                if (keyFrame != value)
                {
                    keyFrame = value;
                    OnPropertyChanged();
                }
            }
        }

        private int inputConfigID;
        public int InputConfigID
        {
            get { return inputConfigID; }
            set
            {
                if (inputConfigID != value)
                {
                    inputConfigID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int pixelSpaceID;
        public int PixelSpaceID
        {
            get { return pixelSpaceID; }
            set
            {
                if (pixelSpaceID != value)
                {
                    pixelSpaceID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string thumbnail;
        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                if (thumbnail != value)
                {
                    thumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        private string windowLabel;
        public string WindowLabel
        {
            get { return windowLabel; }
            set
            {
                if (windowLabel != value)
                {
                    windowLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        private string loadedStill;
        public string LoadedStill
        {
            get { return loadedStill; }
            set
            {
                if (loadedStill != value)
                {
                    loadedStill = value;
                    OnPropertyChanged();
                }
            }
        }

        private string testPattern;
        public string TestPattern
        {
            get { return testPattern; }
            set
            {
                if (testPattern != value)
                {
                    testPattern = value;
                    OnPropertyChanged();
                }
            }
        }

        private double scale;
        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle aoiRect;
        public Rectangle AOIRect
        {
            get { return aoiRect; }
            set
            {
                if (aoiRect != value)
                {
                    aoiRect = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle layerRect;
        public Rectangle LayerRect
        {
            get { return layerRect; }
            set
            {
                if (layerRect != value)
                {
                    layerRect = value;
                    OnPropertyChanged();
                }
            }
        }

        public Rectangle CloneRect
        {
            get
            {
                if (cloneRects == null || cloneRects.Length < 1)
                    return Rectangle.Empty;

                return cloneRects[0];
            }
            set
            {
                if (value.IsEmpty)
                    CloneRects = null;
                else
                    CloneRects = new Rectangle[] { value };
            }
        }

        public Rectangle[] CloneRects
        {
            get { return cloneRects; }
            set
            {
                if(!cloneRects.SequenceEqualSafe(value))
                {
                    cloneRects = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CloneRect));
                }
            }
        }
        private Rectangle[] cloneRects;

        private float aspectRatio;
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                if (aspectRatio != value)
                {
                    aspectRatio = value;
                    OnPropertyChanged();
                }
            }
        }

        private int layerID;
        public int LayerID
        {
            get { return layerID; }
            set
            {
                if (layerID != value)
                {
                    layerID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int hActive;
        public int HActive
        {
            get { return hActive; }
            set
            {
                if (hActive != value)
                {
                    hActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private int vActive;
        public int VActive
        {
            get { return vActive; }
            set
            {
                if (vActive != value)
                {
                    vActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private int priority;
        public int Priority
        {
            get { return priority; }
            set
            {
                if (priority != value)
                {
                    priority = value;
                    OnPropertyChanged();
                }
            }
        }

        private int effectID;
        public int EffectID
        {
            get { return effectID; }
            set
            {
                if (effectID != value)
                {
                    effectID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int lastScript;
        public int LastScript
        {
            get { return lastScript; }
            set
            {
                if (lastScript != value)
                {
                    lastScript = value;
                    OnPropertyChanged();
                }
            }
        }

        private int lastCue;
        public int LastCue
        {
            get { return lastCue; }
            set
            {
                if (lastCue != value)
                {
                    lastCue = value;
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
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        public bool IsVisible
        {
            get { return transparency < 255; }
        }

        private bool isFrozen;
        public bool IsFrozen
        {
            get { return isFrozen; }
            set
            {
                if (isFrozen != value)
                {
                    isFrozen = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isMixer;
        public bool IsMixer
        {
            get { return isMixer; }
            set
            {
                if (isMixer != value)
                {
                    isMixer = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isMixing;
        public bool IsMixing
        {
            get { return isMixing; }
            set
            {
                if (isMixing != value)
                {
                    isMixing = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isSlave;
        public bool IsSlave
        {
            get { return isSlave; }
            set
            {
                if (isSlave != value)
                {
                    isSlave = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool alwaysRelative;
        public bool AlwaysRelative
        {
            get { return alwaysRelative; }
            set
            {
                if (alwaysRelative != value)
                {
                    alwaysRelative = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool autoSyncOnTimingChange;
        public bool AutoSyncOnTimingChange
        {
            get { return autoSyncOnTimingChange; }
            set
            {
                if (autoSyncOnTimingChange != value)
                {
                    autoSyncOnTimingChange = value;
                    OnPropertyChanged();
                }
            }
        }

        private InputStereoMode stereoMode;
        public InputStereoMode StereoMode
        {
            get { return stereoMode; }
            set
            {
                if (stereoMode != value)
                {
                    stereoMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isHardwarePreview;
        public bool IsHardwarePreview
        {
            get { return isHardwarePreview; }
            set
            {
                if (isHardwarePreview != value)
                {
                    isHardwarePreview = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isBackground;
        public bool IsBackground
        {
            get { return isBackground; }
            set
            {
                if (isBackground != value)
                {
                    isBackground = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isWithinPixelSpace;
        public bool IsWithinPixelSpace
        {
            get { return isWithinPixelSpace; }
            set
            {
                if (isWithinPixelSpace != value)
                {
                    isWithinPixelSpace = value;
                    OnPropertyChanged();
                }
            }
        }

        private int frameID;
        public int FrameID
        {
            get { return frameID; }
            set
            {
                if (frameID != value)
                {
                    frameID = value;
                    OnPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Layer {0}: {1}", LayerID - 1, WindowLabel);
        }

        public virtual void CopyFrom(DrawingKeyFrame copyFrom)
        {
            this.AlwaysRelative = copyFrom.AlwaysRelative;
            this.AOIRect = copyFrom.AOIRect;
            this.AspectRatio = copyFrom.AspectRatio;
            this.AutoSyncOnTimingChange = copyFrom.AutoSyncOnTimingChange;
            this.CloneRect = new Rectangle(copyFrom.CloneRect);
            this.EffectID = copyFrom.EffectID;
            this.FrameID = copyFrom.FrameID;
            this.HActive = copyFrom.HActive;
            this.HdcpAuthenticated = copyFrom.HdcpAuthenticated;
            this.InputConfigID = copyFrom.InputConfigID;
            this.IsBackground = copyFrom.IsBackground;
            this.IsFrozen = copyFrom.IsFrozen;
            this.IsHardwarePreview = copyFrom.IsHardwarePreview;
            this.IsMixer = copyFrom.IsMixer;
            this.IsMixing = copyFrom.IsMixing;
            this.IsSlave = copyFrom.IsSlave;
            this.IsLocked = copyFrom.IsLocked;
            this.IsWithinPixelSpace = copyFrom.IsWithinPixelSpace;
            this.KeyFrame.CopyFrom(copyFrom.KeyFrame);
            this.LastCue = copyFrom.LastCue;
            this.LastScript = copyFrom.LastScript;
            this.LayerID = copyFrom.LayerID;
            this.LayerRect = new Rectangle(copyFrom.LayerRect);
            this.LinearKeyRouterID = copyFrom.LinearKeyRouterID;
            this.LinearKeyRouterInput = copyFrom.LinearKeyRouterInput;
            this.LinearKeySource = copyFrom.LinearKeySource;
            this.LoadedStill = copyFrom.LoadedStill;
            this.PixelSpaceID = copyFrom.PixelSpaceID;
            this.Priority = copyFrom.Priority;
            this.Source = copyFrom.Source;
            this.SourceRouterID = copyFrom.SourceRouterID;
            this.SourceRouterInput = copyFrom.SourceRouterInput;
            this.StereoMode = copyFrom.StereoMode;
            this.Thumbnail = copyFrom.Thumbnail;
            this.Transparency = copyFrom.Transparency;
            this.VActive = copyFrom.VActive;
            this.WindowLabel = copyFrom.WindowLabel;
            this.TestPattern = copyFrom.TestPattern;
            this.Scale = copyFrom.Scale;
            this.ShadowIsEnabled = copyFrom.ShadowIsEnabled;
            this.IsGlobalLayer = copyFrom.IsGlobalLayer;
            this.MixEffectID = copyFrom.MixEffectID;

            if (copyFrom.MixEffect == null)
                this.MixEffect = null;
            else if (this.MixEffect == null)
                this.MixEffect = new DrawingMixEffect(copyFrom.MixEffect);
            else
                this.MixEffect.CopyFrom(copyFrom.MixEffect);
        }
    }
}
