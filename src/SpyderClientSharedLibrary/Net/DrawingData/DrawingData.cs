using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.DrawingData
{
    /// <summary>
    /// Contains realtime information transmitted from a Spyder server
    /// </summary>
    public class DrawingData : PropertyChangedBase
    {
        private Dictionary<int, DrawingPixelSpace> pixelSpaces = new Dictionary<int, DrawingPixelSpace>();
        public Dictionary<int, DrawingPixelSpace> PixelSpaces
        {
            get { return pixelSpaces; }
            set
            {
                if (pixelSpaces != value)
                {
                    pixelSpaces = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, int> previewPixelSpaceIDs = new Dictionary<int, int>();
        public Dictionary<int, int> PreviewPixelSpaceIDs
        {
            get { return previewPixelSpaceIDs; }
            set
            {
                if (previewPixelSpaceIDs != value)
                {
                    previewPixelSpaceIDs = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, int> stereoPixelSpaceIDs = new Dictionary<int, int>();
        public Dictionary<int,int> StereoPixelSpaceIDs
        {
            get { return stereoPixelSpaceIDs; }
            set
            {
                if (stereoPixelSpaceIDs != value)
                {
                    stereoPixelSpaceIDs = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, DrawingOutput> outputs = new Dictionary<int, DrawingOutput>();
        public Dictionary<int, DrawingOutput> Outputs
        {
            get { return outputs; }
            set
            {
                if (outputs != value)
                {
                    outputs = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, DrawingRouter> routers = new Dictionary<int, DrawingRouter>();
        public Dictionary<int, DrawingRouter> Routers
        {
            get { return routers; }
            set
            {
                if (routers != value)
                {
                    routers = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, DrawingKeyFrame> drawingKeyFrames = new Dictionary<int, DrawingKeyFrame>();
        public Dictionary<int, DrawingKeyFrame> DrawingKeyFrames
        {
            get { return drawingKeyFrames; }
            set
            {
                if (drawingKeyFrames != value)
                {
                    drawingKeyFrames = value;
                    OnPropertyChanged("DrawingKeyFrames");
                }
            }
        }

        private Dictionary<int, DrawingKeyFrame> previewDrawingKeyFrames = new Dictionary<int, DrawingKeyFrame>();
        public Dictionary<int, DrawingKeyFrame> PreviewDrawingKeyFrames
        {
            get { return previewDrawingKeyFrames; }
            set
            {
                if (previewDrawingKeyFrames != value)
                {
                    previewDrawingKeyFrames = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, DrawingMachine> machines = new Dictionary<int, DrawingMachine>();
        public Dictionary<int, DrawingMachine> Machines
        {
            get { return machines; }
            set
            {
                if (machines != value)
                {
                    machines = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, DrawingFrame> frames = new Dictionary<int, DrawingFrame>();
        public Dictionary<int, DrawingFrame> Frames
        {
            get { return frames; }
            set
            {
                if (frames != value)
                {
                    frames = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, long> runningScripts = new Dictionary<int, long>();
        public Dictionary<int, long> RunningScripts
        {
            get { return runningScripts; }
            set
            {
                if (runningScripts != value)
                {
                    runningScripts = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<DiagnosticType, DiagnosticStatus> diagnosticWarnings = new Dictionary<DiagnosticType, DiagnosticStatus>();
        public Dictionary<DiagnosticType, DiagnosticStatus> DiagnosticWarnings
        {
            get { return diagnosticWarnings; }
            set
            {
                if (diagnosticWarnings != value)
                {
                    diagnosticWarnings = value;
                    OnPropertyChanged();
                }
            }
        }
                
        private TimeSpan timeOfDay;
        public TimeSpan TimeOfDay
        {
            get { return timeOfDay; }
            set
            {
                if (timeOfDay != value)
                {
                    timeOfDay = value;
                    OnPropertyChanged();
                }
            }
        }

        private MixerBus configBus = MixerBus.Program;
        public MixerBus ConfigBus
        {
            get { return configBus; }
            set
            {
                if(configBus != value)
                {
                    configBus = value;
                    OnPropertyChanged();
                }
            }
        }

        private string configSource;
        public string ConfigSource
        {
            get { return configSource; }
            set
            {
                if (configSource != value)
                {
                    configSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private int configLayer;
        public int ConfigLayer
        {
            get { return configLayer; }
            set
            {
                if (configLayer != value)
                {
                    configLayer = value;
                    OnPropertyChanged();
                }
            }
        }

        private int configOutput;
        public int ConfigOutput
        {
            get { return configOutput; }
            set
            {
                if (configOutput != value)
                {
                    configOutput = value;
                    OnPropertyChanged();
                }
            }
        }

        private FrontPanelDisplayCommand lastFrontPanelMsg;
        public FrontPanelDisplayCommand LastFrontPanelMsg
        {
            get { return lastFrontPanelMsg; }
            set
            {
                if (lastFrontPanelMsg != value)
                {
                    lastFrontPanelMsg = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private string progressString;
        public string ProgressString
        {
            get { return progressString; }
            set
            {
                if (progressString != value)
                {
                    progressString = value;
                    OnPropertyChanged("ProgressString");
                }
            }
        }

        private double percentComplete;
        public double PercentComplete
        {
            get { return percentComplete; }
            set
            {
                if (percentComplete != value)
                {
                    percentComplete = value;
                    OnPropertyChanged("PercentComplete");
                }
            }
        }

        private bool isDataIOIdle;
        public bool IsDataIOIdle
        {
            get { return isDataIOIdle; }
            set
            {
                if (isDataIOIdle != value)
                {
                    isDataIOIdle = value;
                    OnPropertyChanged("IsDataIOIdle");
                }
            }
        }

        private bool isStillServerConnected;
        public bool IsStillServerConnected
        {
            get { return isStillServerConnected; }
            set
            {
                if (isStillServerConnected != value)
                {
                    isStillServerConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isLiveUpdateEnabled;
        public bool IsLiveUpdateEnabled
        {
            get { return isLiveUpdateEnabled; }
            set
            {
                if (isLiveUpdateEnabled != value)
                {
                    isLiveUpdateEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool liveUpdatesTemporarilyDisabled;
        public bool LiveUpdatesTemporarilyDisabled
        {
            get { return liveUpdatesTemporarilyDisabled; }
            set
            {
                if(liveUpdatesTemporarilyDisabled != value)
                {
                    liveUpdatesTemporarilyDisabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? isHdcpEnabled;
        public bool? IsHdcpEnabled
        {
            get { return isHdcpEnabled; }
            set
            {
                if (isHdcpEnabled != value)
                {
                    isHdcpEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isPreviewOnlyScriptingEnabled;
        public bool IsPreviewOnlyScriptingEnabled
        {
            get { return isPreviewOnlyScriptingEnabled; }
            set
            {
                if (isPreviewOnlyScriptingEnabled != value)
                {
                    isPreviewOnlyScriptingEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private FieldRate systemFrameRate;
        public FieldRate SystemFrameRate
        {
            get { return systemFrameRate; }
            set
            {
                if (systemFrameRate != value)
                {
                    systemFrameRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private int dataObjectVersion;
        public int DataObjectVersion
        {
            get { return dataObjectVersion; }
            set
            {
                if (dataObjectVersion != value)
                {
                    dataObjectVersion = value;
                    OnPropertyChanged("DataObjectVersion");
                }
            }
        }

        private DataType dataObjectLastChangeType;
        public DataType DataObjectLastChangeType
        {
            get { return dataObjectLastChangeType; }
            set
            {
                if (dataObjectLastChangeType != value)
                {
                    dataObjectLastChangeType = value;
                    OnPropertyChanged("DataObjectLastChangeType");
                }
            }
        }

        private HardwareType hardwareType;
        public HardwareType HardwareType
        {
            get { return hardwareType; }
            set
            {
                if (hardwareType != value)
                {
                    hardwareType = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool opMonOverlay;
        public bool OpMonOverlay
        {
            get { return opMonOverlay; }
            set
            {
                if (opMonOverlay != value)
                {
                    opMonOverlay = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isLayerZeroPreviewBackground;
        public bool IsLayerZeroPreviewBackground
        {
            get { return isLayerZeroPreviewBackground; }
            set
            {
                if (isLayerZeroPreviewBackground != value)
                {
                    isLayerZeroPreviewBackground = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isMachineHalEnabled;
        public bool IsMachineHalEnabled
        {
            get { return isMachineHalEnabled; }
            set
            {
                if (isMachineHalEnabled != value)
                {
                    isMachineHalEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isRouterHalEnabled;
        public bool IsRouterHalEnabled
        {
            get { return isRouterHalEnabled; }
            set
            {
                if (isRouterHalEnabled != value)
                {
                    isRouterHalEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public DrawingPixelSpace GetPixelSpace(int pixelSpaceID)
        {
            if (pixelSpaces.ContainsKey(pixelSpaceID))
                return pixelSpaces[pixelSpaceID];

            return null;
        }

        public int GetPreviewPixelSpaceID(int programPixelSpaceID)
        {
            if (previewPixelSpaceIDs.ContainsKey(programPixelSpaceID))
                return previewPixelSpaceIDs[programPixelSpaceID];

            return -1;
        }

        public List<DrawingKeyFrame> GetLayers(int pixelSpaceID, bool includeBackgroundLayers = false)
        {
            var ps = GetPixelSpace(pixelSpaceID);
            if (ps == null)
                return new List<DrawingKeyFrame>();

            var layerList = (ps.Scale == 1 ? drawingKeyFrames : previewDrawingKeyFrames);
            var response = layerList.Values.Where(l => l.PixelSpaceID == pixelSpaceID);

            if (!includeBackgroundLayers)
                response = response.Where(l => !l.IsBackground);

            return response.ToList();
        }

        public DrawingKeyFrame GetLayer(int layerIndex, MixerBus bus)
        {
            if (bus == MixerBus.Program)
            {
                if (drawingKeyFrames.ContainsKey(layerIndex))
                    return drawingKeyFrames[layerIndex];
            }

            if (previewDrawingKeyFrames.ContainsKey(layerIndex))
                return previewDrawingKeyFrames[layerIndex];

            //Not found
            return null;
        }

        public void GetCommandKeyState(int scriptID, out bool pvw, out bool pgm, bool checkPreviewLayers = true)
        {
            pvw = false;
            pgm = false;

            //Check program layers
            if (DrawingKeyFrames != null && DrawingKeyFrames.Count != 0)
            {
                foreach (DrawingKeyFrame dkf in DrawingKeyFrames.Values)
                {
                    if (dkf.IsVisible && dkf.LastScript == scriptID)
                    {
                        PixelSpace ps = GetPixelSpace(dkf.PixelSpaceID);
                        if (ps != null)
                        {
                            if (ps.Scale == 1.0f)
                            {
                                pgm = true;
                            }
                            else
                            {
                                pvw = true;
                            }
                        }
                    }

                    // if both are true, no need to check further
                    if (pvw && pgm)
                        return;
                }
            }

            //Check preview layers?
            if (checkPreviewLayers)
            {
                if (PreviewDrawingKeyFrames != null && PreviewDrawingKeyFrames.Count != 0)
                {
                    foreach (DrawingKeyFrame dkf in PreviewDrawingKeyFrames.Values)
                    {
                        if (dkf.IsVisible && dkf.LastScript == scriptID)
                        {
                            PixelSpace ps = GetPixelSpace(dkf.PixelSpaceID);
                            if (ps != null)
                            {
                                if (ps.Scale == 1.0f)
                                {
                                    pgm = true;
                                }
                                else
                                {
                                    pvw = true;
                                }
                            }
                        }

                        // if both are true, no need to check further
                        if (pvw && pgm)
                            return;
                    }
                }
            }
        }
    }
}
