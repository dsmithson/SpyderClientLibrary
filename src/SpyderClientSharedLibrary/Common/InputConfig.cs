using Spyder.Client.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class InputConfig : PropertyChangedBase
    {
        private int id;
        public int ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        private ColorSpace colorSpace;
        public ColorSpace ColorSpace
        {
            get { return colorSpace; }
            set
            {
                if (colorSpace != value)
                {
                    colorSpace = value;
                    OnPropertyChanged();
                }
            }
        }

        private float verticalFrequency;
        public float VerticalFrequency
        {
            get { return verticalFrequency; }
            set
            {
                if (verticalFrequency != value)
                {
                    verticalFrequency = value;
                    OnPropertyChanged();
                }
            }
        }

        private int vTotal;
        public int VTotal
        {
            get { return vTotal; }
            set
            {
                if (vTotal != value)
                {
                    vTotal = value;
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

        private int hTotal;
        public int HTotal
        {
            get { return hTotal; }
            set
            {
                if (hTotal != value)
                {
                    hTotal = value;
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

        private bool isInterlaced;
        public bool IsInterlaced
        {
            get { return isInterlaced; }
            set
            {
                if (isInterlaced != value)
                {
                    isInterlaced = value;
                    OnPropertyChanged();
                }
            }
        }

        private float noiseRed;
        public float NoiseRed
        {
            get { return noiseRed; }
            set
            {
                if (noiseRed != value)
                {
                    noiseRed = value;
                    OnPropertyChanged();
                }
            }
        }

        private float detailEnhance;
        public float DetailEnhance
        {
            get { return detailEnhance; }
            set
            {
                if (detailEnhance != value)
                {
                    detailEnhance = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isVMotionDetect;
        public bool IsVMotionDetect
        {
            get { return isVMotionDetect; }
            set
            {
                if (isVMotionDetect != value)
                {
                    isVMotionDetect = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isEdgeDetect;
        public bool IsEdgeDetect
        {
            get { return isEdgeDetect; }
            set
            {
                if (isEdgeDetect != value)
                {
                    isEdgeDetect = value;
                    OnPropertyChanged();
                }
            }
        }

        private int sogPickoff;
        public int SOGPickoff
        {
            get { return sogPickoff; }
            set
            {
                if (sogPickoff != value)
                {
                    sogPickoff = value;
                    OnPropertyChanged();
                }
            }
        }

        private int vStart;
        public int VStart
        {
            get { return vStart; }
            set
            {
                if (vStart != value)
                {
                    vStart = value;
                    OnPropertyChanged();
                }
            }
        }

        private int hStart;
        public int HStart
        {
            get { return hStart; }
            set
            {
                if (hStart != value)
                {
                    hStart = value;
                    OnPropertyChanged();
                }
            }
        }

        private int clockPhase;
        public int ClockPhase
        {
            get { return clockPhase; }
            set
            {
                if (clockPhase != value)
                {
                    clockPhase = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool useManualPllClockPhase;
        public bool UseManualPLLClockPhase
        {
            get { return useManualPllClockPhase; }
            set
            {
                if (useManualPllClockPhase != value)
                {
                    useManualPllClockPhase = value;
                    OnPropertyChanged();
                }
            }
        }

        private int pllClockPhase;
        public int PLLClockPhase
        {
            get { return pllClockPhase; }
            set
            {
                if (pllClockPhase != value)
                {
                    pllClockPhase = value;
                    OnPropertyChanged();
                }
            }
        }

        private int dcRestorePulseWidth;
        public int DCRestorePulseWidth
        {
            get { return dcRestorePulseWidth; }
            set
            {
                if (dcRestorePulseWidth != value)
                {
                    dcRestorePulseWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        private int dcRestorePulseDelay;
        public int DCRestorePulseDelay
        {
            get { return dcRestorePulseDelay; }
            set
            {
                if (dcRestorePulseDelay != value)
                {
                    dcRestorePulseDelay = value;
                    OnPropertyChanged();
                }
            }
        }

        private int hBackPorch;
        public int HBackPorch
        {
            get { return hBackPorch; }
            set
            {
                if (hBackPorch != value)
                {
                    hBackPorch = value;
                    OnPropertyChanged();
                }
            }
        }

        private DCRestoreMode dcRestoreMode;
        public DCRestoreMode DCRestoreMode
        {
            get { return dcRestoreMode; }
            set
            {
                if (dcRestoreMode != value)
                {
                    dcRestoreMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private InputSyncType syncType;
        public InputSyncType SyncType
        {
            get { return syncType; }
            set
            {
                if (syncType != value)
                {
                    syncType = value;
                    OnPropertyChanged();
                }
            }
        }

        private InputConnector connectorType;
        public InputConnector ConnectorType
        {
            get { return connectorType; }
            set
            {
                if (connectorType != value)
                {
                    connectorType = value;
                    OnPropertyChanged();
                }
            }
        }

        private DCPulsePos dcPulsePosition;
        public DCPulsePos DCPulsePosition
        {
            get { return dcPulsePosition; }
            set
            {
                if (dcPulsePosition != value)
                {
                    dcPulsePosition = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool useAlternateInputSynchronizationMethod;
        public bool UseAlternateInputSynchronizationMethod
        {
            get { return useAlternateInputSynchronizationMethod; }
            set
            {
                if (useAlternateInputSynchronizationMethod != value)
                {
                    useAlternateInputSynchronizationMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        private float contrast;
        public float Contrast
        {
            get { return contrast; }
            set
            {
                if (contrast != value)
                {
                    contrast = value;
                    OnPropertyChanged();
                }
            }
        }

        private float brightness;
        public float Brightness
        {
            get { return brightness; }
            set
            {
                if (brightness != value)
                {
                    brightness = value;
                    OnPropertyChanged();
                }
            }
        }

        private float hue;
        public float Hue
        {
            get { return hue; }
            set
            {
                if (hue != value)
                {
                    hue = value;
                    OnPropertyChanged();
                }
            }
        }

        private float saturation;
        public float Saturation
        {
            get { return saturation; }
            set
            {
                if (saturation != value)
                {
                    saturation = value;
                    OnPropertyChanged();
                }
            }
        }

        private int hHoldOff;
        public int HHoldOff
        {
            get { return hHoldOff; }
            set
            {
                if (hHoldOff != value)
                {
                    hHoldOff = value;
                    OnPropertyChanged();
                }
            }
        }

        private int vHoldOff;
        public int VHoldOff
        {
            get { return vHoldOff; }
            set
            {
                if (vHoldOff != value)
                {
                    vHoldOff = value;
                    OnPropertyChanged();
                }
            }
        }

        private int vDelay;
        public int VDelay
        {
            get { return vDelay; }
            set
            {
                if (vDelay != value)
                {
                    vDelay = value;
                    OnPropertyChanged();
                }
            }
        }

        private int hDelay;
        public int HDelay
        {
            get { return hDelay; }
            set
            {
                if (hDelay != value)
                {
                    hDelay = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool stereoInvertEyes;
        public bool StereoInvertEyes
        {
            get { return stereoInvertEyes; }
            set
            {
                if (stereoInvertEyes != value)
                {
                    stereoInvertEyes = value;
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

        private bool generateStereoSyncWhenMissing;
        public bool GenerateStereoSyncWhenMissing
        {
            get { return generateStereoSyncWhenMissing; }
            set
            {
                if (generateStereoSyncWhenMissing != value)
                {
                    generateStereoSyncWhenMissing = value;
                    OnPropertyChanged();
                }
            }
        }

        private int stereoElevation;
        public int StereoElevation
        {
            get { return stereoElevation; }
            set
            {
                if (stereoElevation != value)
                {
                    stereoElevation = value;
                    OnPropertyChanged();
                }
            }
        }

        private KeyerMode keyerMode;
        public KeyerMode KeyerMode
        {
            get { return keyerMode; }
            set
            {
                if (keyerMode != value)
                {
                    keyerMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color keyColor;
        public Color KeyColor
        {
            get { return keyColor; }
            set
            {
                if (keyColor != value)
                {
                    keyColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte keyRedWindow;
        public byte KeyRedWindow
        {
            get { return keyRedWindow; }
            set
            {
                if (keyRedWindow != value)
                {
                    keyRedWindow = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte keyGreenWindow;
        public byte KeyGreenWindow
        {
            get { return keyGreenWindow; }
            set
            {
                if (keyGreenWindow != value)
                {
                    keyGreenWindow = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte keyBlueWindow;
        public byte KeyBlueWindow
        {
            get { return keyBlueWindow; }
            set
            {
                if (keyBlueWindow != value)
                {
                    keyBlueWindow = value;
                    OnPropertyChanged();
                }
            }
        }

        private int keyColorGain;
        public int KeyColorGain
        {
            get { return keyColorGain; }
            set
            {
                if (keyColorGain != value)
                {
                    keyColorGain = value;
                    OnPropertyChanged();
                }
            }
        }

        private int keyGain;
        public int KeyGain
        {
            get { return keyGain; }
            set
            {
                if (keyGain != value)
                {
                    keyGain = value;
                    OnPropertyChanged();
                }
            }
        }

        private int keyClip;
        public int KeyClip
        {
            get { return keyClip; }
            set
            {
                if (keyClip != value)
                {
                    keyClip = value;
                    OnPropertyChanged();
                }
            }
        }

        private float gamma;
        public float Gamma
        {
            get { return gamma; }
            set
            {
                if (gamma != value)
                {
                    gamma = value;
                    OnPropertyChanged();
                }
            }
        }

        private int cropOffsetTop;
        public int CropOffsetTop
        {
            get { return cropOffsetTop; }
            set
            {
                if (cropOffsetTop != value)
                {
                    cropOffsetTop = value;
                    OnPropertyChanged();
                }
            }
        }

        private int cropOffsetLeft;
        public int CropOffsetLeft
        {
            get { return cropOffsetLeft; }
            set
            {
                if (cropOffsetLeft != value)
                {
                    cropOffsetLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        private int cropOffsetRight;
        public int CropOffsetRight
        {
            get { return cropOffsetRight; }
            set
            {
                if (cropOffsetRight != value)
                {
                    cropOffsetRight = value;
                    OnPropertyChanged();
                }
            }
        }

        private int cropOffsetBottom;
        public int CropOffsetBottom
        {
            get { return cropOffsetBottom; }
            set
            {
                if (cropOffsetBottom != value)
                {
                    cropOffsetBottom = value;
                    OnPropertyChanged();
                }
            }
        }

        private AutoSyncMode autoSyncMode;
        public AutoSyncMode AutoSyncMode
        {
            get { return autoSyncMode; }
            set
            {
                if (autoSyncMode != value)
                {
                    autoSyncMode = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
