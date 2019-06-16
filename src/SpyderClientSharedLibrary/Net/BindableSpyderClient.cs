using Spyder.Client.Common;
using Knightware.Diagnostics;
using Knightware.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spyder.Client.Net.Notifications;
using System.Diagnostics;
using Spyder.Client.Scripting;
using Spyder.Client.FunctionKeys;
using Knightware.Primitives;
using Spyder.Client.Net.DrawingData;

namespace Spyder.Client.Net
{
    /// <summary>
    /// Wraps a SpyderClient instance and provides additional bindable collections from a Spyder server
    /// </summary>
    public class BindableSpyderClient : DispatcherPropertyChangedBase, ISpyderClientExtended, IComparable
    {
        private const int maxTraceMessages = 100;
        private readonly ISpyderClientExtended client;
        private int lastDataObjectVersion = -1;

        #region Properties

        public HardwareType HardwareType
        {
            get { return client.HardwareType; }
        }

        /// <summary>
        /// Contains path information to access files on the Spyder server
        /// </summary>
        public ServerFilePaths ServerFilePaths => client?.ServerFilePaths;

        /// <summary>
        /// Defines a throttle for maximum drawing data event raising (per Spyder server).  Setting to 1 second, for example, will ensure DrawingData does not fire more than once per second.  Set to TimeSpan.Zero (default) to disable throttling.
        /// </summary>
        public TimeSpan DrawingDataThrottleInterval
        {
            get { return drawingDataThrottleInterval; }
            set
            {
                if (drawingDataThrottleInterval != value)
                {
                    drawingDataThrottleInterval = value;
                    OnPropertyChanged();
                }

                client.DrawingDataThrottleInterval = value;
            }
        }
        private TimeSpan drawingDataThrottleInterval = TimeSpan.Zero;

        private DrawingData.DrawingData drawingData;
        public DrawingData.DrawingData DrawingData
        {
            get { return drawingData; }
            set
            {
                if (drawingData != value)
                {
                    drawingData = value;
                    OnPropertyChanged();
                }
            }
        }

        private DiagnosticStatus userDiagnosticOverallStatus = DiagnosticStatus.Unknown;
        public DiagnosticStatus UserDiagnosticOverallStatus
        {
            get { return userDiagnosticOverallStatus; }
            set
            {
                if (userDiagnosticOverallStatus != value)
                {
                    userDiagnosticOverallStatus = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public ObservableCollection<TraceMessage> TraceMessages => new ObservableCollection<TraceMessage>();

        private TracingLevel traceMessagesOverallLevel = TracingLevel.Success;
        public TracingLevel TraceMessagesOverallLevel
        {
            get { return traceMessagesOverallLevel; }
            set
            {
                if (traceMessagesOverallLevel != value)
                {
                    traceMessagesOverallLevel = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CommandKey> commandKeys;
        public ObservableCollection<CommandKey> CommandKeys
        {
            get
            {
                if (commandKeys == null)
                {
                    //Start a pull for command keys.  This will update this collection when it completes
                    GetCommandKeys();
                }
                return commandKeys;
            }
            protected set
            {
                if (commandKeys != value)
                {
                    commandKeys = value;
                    OnPropertyChanged("CommandKeys");
                }
            }
        }

        private ObservableCollection<Script> scripts;
        public ObservableCollection<Script> Scripts
        {
            get
            {
                if (scripts == null)
                {
                    //Start a pull for scripts.  Thsi will update this collection when it completes
                    GetScripts();
                }
                return scripts;
            }
            set
            {
                if (scripts != value)
                {
                    scripts = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<FunctionKey> functionKeys;
        public ObservableCollection<FunctionKey> FunctionKeys
        {
            get
            {
                if (functionKeys == null)
                {
                    //Start a pull for function keys.  This will update this collection when it completes
                    GetFunctionKeys();
                }
                return functionKeys;
            }
            protected set
            {
                if (functionKeys != value)
                {
                    functionKeys = value;
                    OnPropertyChanged("FunctionKeys");
                }
            }
        }

        private ObservableCollection<Source> sources;
        public ObservableCollection<Source> Sources
        {
            get
            {
                if (sources == null)
                {
                    //Start a pull for sources.  This will update this collection when it completes
                    GetSources();
                }
                return sources;
            }
            protected set
            {
                if (sources != value)
                {
                    sources = value;
                    OnPropertyChanged("Sources");
                }
            }
        }

        private ObservableCollection<Treatment> treatments;
        public ObservableCollection<Treatment> Treatments
        {
            get
            {
                if (treatments == null)
                {
                    //Start a pull for treatments.  This will update this collection when it completes
                    GetTreatments();
                }
                return treatments;
            }
            protected set
            {
                if (treatments != value)
                {
                    treatments = value;
                    OnPropertyChanged("Treatments");
                }
            }
        }

        private ObservableCollection<Still> stills;
        public ObservableCollection<Still> Stills
        {
            get
            {
                if (stills == null)
                {
                    //Start a pull for stills.  This will update this collection when it completes
                    GetStills();
                }
                return stills;
            }
            set
            {
                if (stills != value)
                {
                    stills = value;
                    OnPropertyChanged("Stills");
                }
            }
        }

        private ObservableCollection<PlayItem> playItems;
        public ObservableCollection<PlayItem> PlayItems
        {
            get
            {
                if (playItems == null)
                {
                    //Start a pull for playItems.  This will update this collection when it completes
                    GetPlayItems();
                }
                return playItems;
            }
            protected set
            {
                if (playItems != value)
                {
                    playItems = value;
                    OnPropertyChanged("PlayItems");
                }
            }
        }

        private ObservableCollection<PixelSpace> pixelSpaces;
        public ObservableCollection<PixelSpace> PixelSpaces
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

        private ObservableCollection<InputConfig> inputConfigs;
        public ObservableCollection<InputConfig> InputConfigs
        {
            get { return inputConfigs; }
            set
            {
                if (inputConfigs != value)
                {
                    inputConfigs = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public BindableSpyderClient(ISpyderClientExtended client)
        {
            this.client = client;
        }

        public Task<bool> Save()
        {
            return client.Save();
        }

        public async Task<bool> LoadDataAsync()
        {
            //Refresh source data, if possible
            var spyderClient = client as SpyderClient;
            if (spyderClient != null)
            {
                if (!await spyderClient.LoadDataAsync())
                    return false;
            }

            //Update our internal collections by calling their general data getters
            var tasks = new List<Task>();
            if (commandKeys != null)
                tasks.Add(GetCommandKeys());

            if (scripts != null)
                tasks.Add(GetScripts());

            if (functionKeys != null)
                tasks.Add(GetFunctionKeys());

            if (sources != null)
                tasks.Add(GetSources());

            if (treatments != null)
                tasks.Add(GetTreatments());

            if (stills != null)
                tasks.Add(GetStills());

            if (playItems != null)
                tasks.Add(GetPlayItems());

            if (pixelSpaces != null)
                tasks.Add(GetPixelSpaces());

            if (inputConfigs != null)
                tasks.Add(GetInputConfigs());

            //Wait for any pending operations above to complete
            if (tasks.Count > 0)
                await Task.WhenAll(tasks);

            return true;
        }

        #region ISpyderClient interface wrapper

        public event DataObjectChangedHandler DataObjectChanged;
        protected void OnDataObjectChanged(DataObjectChangedEventArgs e)
        {
            if (DataObjectChanged != null)
                DataObjectChanged(this, e);
        }

        public event DrawingDataReceivedHandler DrawingDataReceived;
        protected void OnDrawingDataReceived(DrawingDataReceivedEventArgs e)
        {
            if (DrawingDataReceived != null)
                DrawingDataReceived(this, e);
        }

        public event TraceLogMessageHandler TraceLogMessageReceived;
        protected void OnTraceLogMessageReceived(TraceLogMessageEventArgs e)
        {
            if (TraceLogMessageReceived != null)
                TraceLogMessageReceived(this, e);
        }

        public bool IsRunning
        {
            get { return client.IsRunning; }
        }

        public string ServerIP
        {
            get { return client.ServerIP; }
        }

        public string ServerName
        {
            get
            {
                if (client is SpyderClient)
                    return ((SpyderClient)client).ServerName;
                else
                    return null;
            }
        }

        public string ServerDisplayName
        {
            get
            {
                string serverName = this.ServerName;
                if (string.IsNullOrEmpty(serverName))
                    return this.ServerIP;
                else
                    return serverName;
            }
        }

        public async Task<bool> StartupAsync()
        {
            await ShutdownAsync();

            //Hook for trace messages so we can see messages related to connections from the server side
            client.TraceLogMessageReceived += client_TraceLogMessageReceived;

            if (!await client.StartupAsync())
            {
                await ShutdownAsync();
                return false;
            }

            //Do additional initialization here
            client.DrawingDataReceived += client_DrawingDataReceived;
            client.DataObjectChanged += client_DataObjectChanged;

            return true;
        }

        public async Task ShutdownAsync()
        {
            client.TraceLogMessageReceived -= client_TraceLogMessageReceived;
            client.DrawingDataReceived -= client_DrawingDataReceived;
            client.DataObjectChanged -= client_DataObjectChanged;

            await client.ShutdownAsync();

            this.TraceMessages.Clear();

            drawingData = null;
            lastDataObjectVersion = -1;
            this.commandKeys = null;
            this.functionKeys = null;
            this.sources = null;
            this.stills = null;
            this.inputConfigs = null;
            this.pixelSpaces = null;
            this.playItems = null;
            this.scripts = null;
            this.treatments = null;
            this.userDiagnosticOverallStatus = DiagnosticStatus.Unknown;
        }

        private void EvaluateAndRaiseDataObjectChanged(int dataObjectVersion, DataType changedDataTypes)
        {
            if (!IsRunning || dataObjectVersion == lastDataObjectVersion)
                return;

            //If we've missed a data object version, then there may be more than one type of data changed.  Upgrade change types to all
            if (dataObjectVersion - lastDataObjectVersion != 1 && lastDataObjectVersion != -1)
                changedDataTypes = DataType.All;

            lastDataObjectVersion = dataObjectVersion;
            OnDataObjectChanged(new DataObjectChangedEventArgs(ServerIP, dataObjectVersion, changedDataTypes));
        }

        private void client_DataObjectChanged(object sender, DataObjectChangedEventArgs e)
        {
            EvaluateAndRaiseDataObjectChanged(e.Version, e.ChangedDataTypes);
        }

        void client_DrawingDataReceived(object sender, DrawingDataReceivedEventArgs e)
        {
            if (!IsRunning || e.DrawingData == null)
                return;

            try
            {
                var drawingData = e.DrawingData;

                //Update overall status
                if (drawingData.DiagnosticWarnings == null || drawingData.DiagnosticWarnings.Count == 0)
                {
                    this.userDiagnosticOverallStatus = DiagnosticStatus.NotRun;
                }
                else
                {
                    this.UserDiagnosticOverallStatus = drawingData.DiagnosticWarnings.Values.Min();
                }

                //Check dataobject changed.  It's a good check here even though we get an explicit dataObjectChanged message
                //from the server, because it's possible that we'll miss the UDP message
                EvaluateAndRaiseDataObjectChanged(e.DrawingData.DataObjectVersion, e.DrawingData.DataObjectLastChangeType);

                this.DrawingData = e.DrawingData;
                OnDrawingDataReceived(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} occurred while raising drawingdata changed event: {1}", ex.GetType().Name, ex.Message);
            }
        }

        void client_TraceLogMessageReceived(object sender, TraceLogMessageEventArgs e)
        {
            if (!IsRunning)
                return;

            try
            {
                //Add to my local cache and trim extra messages
                TraceMessages.Insert(0, e.Message);
                if (e.Message.Level < this.TraceMessagesOverallLevel)
                    this.TraceMessagesOverallLevel = e.Message.Level;

                bool updateSeverity = false;
                while (TraceMessages.Count > maxTraceMessages)
                {
                    var msgToRemove = TraceMessages[maxTraceMessages - 1];
                    TraceMessages.RemoveAt(maxTraceMessages - 1);
                    if (msgToRemove.Level == this.traceMessagesOverallLevel)
                        updateSeverity = true;
                }
                if (updateSeverity)
                {
                    this.TraceMessagesOverallLevel = TraceMessages.Min(msg => msg.Level);
                }

                //Raise event
                OnTraceLogMessageReceived(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} occurred while raising TraceLogMessageReceived event: {1}", ex.GetType().Name, ex.Message);
            }
        }

        public Task<Stream> GetImageFileStream(string fileName, int? maxWidthOrHeight = 2048)
        {
            return client.GetImageFileStream(fileName);
        }

        public Task<bool> GetImageFileStream(string fileName, Stream targetStream, int? maxWidthOrHeight = 2048)
        {
            return client.GetImageFileStream(fileName, targetStream, maxWidthOrHeight);
        }

        public Task<bool> SetImageFileStream(string fileName, Stream fileStream)
        {
            return client.SetImageFileStream(fileName, fileStream);
        }

        public Task<List<RegisterPage>> GetRegisterPages(RegisterType type)
        {
            return client.GetRegisterPages(type);
        }
        public Task<List<IRegister>> GetRegisters(RegisterType type)
        {
            return client.GetRegisters(type);
        }
        public Task<IRegister> GetRegister(RegisterType type, int registerID)
        {
            return client.GetRegister(type, registerID);
        }

        public Task<List<Source>> GetSources()
        {
            var task = client.GetSources();
            task.ContinueWith((t) => UpdateList(t, (list) => this.Sources = list));
            return task;
        }
        public Task<Source> GetSource(int sourceRegisterID)
        {
            return client.GetSource(sourceRegisterID);
        }
        public Task<Source> GetSource(IRegister sourceRegister)
        {
            return client.GetSource(sourceRegister);
        }

        public Task<Source> GetSource(string sourceName)
        {
            return client.GetSource(sourceName);
        }

        public Task<List<CommandKey>> GetCommandKeys()
        {
            var task = client.GetCommandKeys();
            task.ContinueWith((t) => UpdateList(t, (list) => this.CommandKeys = list));
            return task;
        }
        public Task<CommandKey> GetCommandKey(int commandKeyRegisterID)
        {
            return client.GetCommandKey(commandKeyRegisterID);
        }
        public Task<CommandKey> GetCommandKey(IRegister commandKeyRegister)
        {
            return client.GetCommandKey(commandKeyRegister);
        }

        public Task<List<Script>> GetScripts()
        {
            var task = client.GetScripts();
            task.ContinueWith((t) => UpdateList(t, (list) => this.Scripts = list));
            return task;
        }

        public Task<Script> GetScript(int scriptID)
        {
            return client.GetScript(scriptID);
        }

        public Task<bool> DeleteCommandKey(params int[] commandKeyRegisterIDs)
        {
            return client.DeleteCommandKey(commandKeyRegisterIDs);
        }

        public Task<int> GetRunningCommandKeyCue(int registerID)
        {
            return client.GetRunningCommandKeyCue(registerID);
        }

        public Task<List<Treatment>> GetTreatments()
        {
            var task = client.GetTreatments();
            task.ContinueWith((t) => UpdateList(t, (list) => this.Treatments = list));
            return task;
        }
        public Task<Treatment> GetTreatment(int treatmentRegisterID)
        {
            return client.GetTreatment(treatmentRegisterID);
        }
        public Task<Treatment> GetTreatment(IRegister treatmentRegister)
        {
            return client.GetTreatment(treatmentRegister);
        }

        public Task<List<FunctionKey>> GetFunctionKeys()
        {
            var task = client.GetFunctionKeys();
            task.ContinueWith((t) => UpdateList(t, (list) => this.FunctionKeys = list));
            return task;
        }
        public Task<FunctionKey> GetFunctionKey(int functionKeyRegisterID)
        {
            return client.GetFunctionKey(functionKeyRegisterID);
        }
        public Task<FunctionKey> GetFunctionKey(IRegister functionKeyRegister)
        {
            return client.GetFunctionKey(functionKeyRegister);
        }

        public Task<List<Still>> GetStills()
        {
            var task = client.GetStills();
            task.ContinueWith((t) => UpdateList(t, (list) => this.Stills = list));
            return task;
        }
        public Task<Still> GetStill(int stillRegisterID)
        {
            return client.GetStill(stillRegisterID);
        }
        public Task<Still> GetStill(IRegister stillRegister)
        {
            return client.GetStill(stillRegister);
        }

        public Task<List<PlayItem>> GetPlayItems()
        {
            var task = client.GetPlayItems();
            task.ContinueWith((t) => UpdateList(t, (list) => this.PlayItems = list));
            return task;
        }
        public Task<PlayItem> GetPlayItem(int playItemRegisterID)
        {
            return client.GetPlayItem(playItemRegisterID);
        }
        public Task<PlayItem> GetPlayItem(IRegister playItemRegister)
        {
            return client.GetPlayItem(playItemRegister);
        }

        public Task<List<PixelSpace>> GetPixelSpaces()
        {
            var task = client.GetPixelSpaces();
            task.ContinueWith((t) => UpdateList(t, (list) => this.PixelSpaces = list));
            return task;
        }

        public Task<PixelSpace> GetPixelSpace(int pixelSpaceID)
        {
            return client.GetPixelSpace(pixelSpaceID);
        }

        public Task<List<InputConfig>> GetInputConfigs()
        {
            var task = client.GetInputConfigs();
            task.ContinueWith((t) => UpdateList(t, (list) => this.InputConfigs = list));
            return task;
        }

        public Task<InputConfig> GetInputConfig(int inputConfigID)
        {
            return client.GetInputConfig(inputConfigID);
        }

        public Task<InputConfig> GetInputConfig(string sourceName)
        {
            return client.GetInputConfig(sourceName);
        }

        public Task<bool> LearnCommandKey(int pageIndex, int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative)
        {
            return client.LearnCommandKey(pageIndex, registerID, name, learnFrom, learnAsMixers, learnAsRelative);
        }

        public Task<bool> LearnCommandKey(int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative)
        {
            return client.LearnCommandKey(registerID, name, learnFrom, learnAsMixers, learnAsRelative);
        }

        public Task<bool> RecallCommandKey(int registerID, int cueIndex)
        {
            return client.RecallCommandKey(registerID, cueIndex);
        }

        public Task<bool> RecallFunctionKey(int registerID)
        {
            return client.RecallFunctionKey(registerID);
        }

        public Task<bool> RecallRegisterToLayer(RegisterType registerType, int registerID, params int[] layerIDs)
        {
            return client.RecallRegisterToLayer(registerType, registerID, layerIDs);
        }

        #endregion

        /// <summary>
        /// Updates a collection using the update method action parameter, using a provided task result dispatched to the UI thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newCollectionTask"></param>
        /// <param name="updateMethod"></param>
        private void UpdateList<T>(Task<List<T>> newCollectionTask, Action<ObservableCollection<T>> updateMethod)
        {
            if (newCollectionTask == null || newCollectionTask.Exception != null || newCollectionTask.Status != TaskStatus.RanToCompletion)
                return;

            ObservableCollection<T> newCollection = null;
            if (newCollectionTask.Result != null)
            {
                newCollection = new ObservableCollection<T>(newCollectionTask.Result);
            }

            updateMethod(newCollection);
        }

        public Task<List<Shape>> GetShapes()
        {
            return client.GetShapes();
        }

        public Task<Shape> GetShape(string shapeFileName)
        {
            return client.GetShape(shapeFileName);
        }

        public Task<bool> SetShape(Shape shape)
        {
            return client.SetShape(shape);
        }

        public Task<List<string>> GetShapeFileNames()
        {
            return client.GetShapeFileNames();
        }

        public Task<VersionInfo> GetVersionInfo()
        {
            return client.GetVersionInfo();
        }

        public Task<ServerSettings> GetServerSettings()
        {
            return client.GetServerSettings();
        }

        public Task<bool> MixBackground(int duration)
        {
            return client.MixBackground(duration);
        }

        #region Layer Interaction

        public Task<List<KeyframePropertyValue>> KeyframePropertiesGet(int layerID)
        {
            return client.KeyframePropertiesGet(layerID);
        }

        public Task<bool> KeyframePropertySet(int layerID, string propertyName, object value)
        {
            return client.KeyframePropertySet(layerID, propertyName, value);
        }

        public Task<bool> KeyframePropertySet(int layerID, Dictionary<string, object> propertiesAndValues)
        {
            return client.KeyframePropertySet(layerID, propertiesAndValues);
        }

        public Task<int> GetLayerCount()
        {
            return client.GetLayerCount();
        }

        public Task<int> GetFirstAvailableLayerID()
        {
            return client.GetFirstAvailableLayerID();
        }

        public Task<bool> ApplyRegisterToLayer(RegisterType type, int registerID, params int[] layerIDs)
        {
            return client.ApplyRegisterToLayer(type, registerID, layerIDs);
        }

        public Task<bool> FreezeLayer(params int[] layerIDs)
        {
            return client.FreezeLayer(layerIDs);
        }

        public Task<bool> UnFreezeLayer(params int[] layerIDs)
        {
            return client.UnFreezeLayer(layerIDs);
        }

        public Task<bool> MixOffAllLayers(TimeSpan duration)
        {
            return client.MixOffAllLayers(duration);
        }

        public Task<bool> MixOffAllLayers(int duration)
        {
            return client.MixOffAllLayers(duration);
        }

        public Task<bool> MixOffLayer(TimeSpan duration, params int[] layerIDs)
        {
            return client.MixOffLayer(duration, layerIDs);
        }

        public Task<bool> MixOffLayer(int duration, params int[] layerIDs)
        {
            return client.MixOffLayer(duration, layerIDs);
        }

        public Task<bool> MixOnLayer(TimeSpan duration, params int[] layerIDs)
        {
            return client.MixOnLayer(duration, layerIDs);
        }

        public Task<bool> MixOnLayer(int duration, params int[] layerIDs)
        {
            return client.MixOnLayer(duration, layerIDs);
        }

        public Task<bool> MixOnLayer(int pixelSpaceID, Point position, int width, TimeSpan duration, Register content)
        {
            return client.MixOnLayer(pixelSpaceID, position, width, duration, content);
        }

        public Task<bool> MixOnLayer(int pixelSpaceID, Point position, int width, int duration, Register content)
        {
            return client.MixOnLayer(pixelSpaceID, position, width, duration, content);
        }

        public Task<bool> MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, TimeSpan duration, Register content)
        {
            return client.MixOnLayer(layerID, pixelSpaceID, position, width, duration, content);
        }

        public Task<bool> MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, int duration, Register content)
        {
            return client.MixOnLayer(layerID, pixelSpaceID, position, width, duration, content);
        }

        public Task<bool> TransitionLayer(TimeSpan duration, params int[] layerIDs)
        {
            return client.TransitionLayer(duration, layerIDs);
        }

        public Task<bool> TransitionLayer(int duration, params int[] layerIDs)
        {
            return client.TransitionLayer(duration, layerIDs);
        }

        public Task<bool> ResizeLayer(LayerResizeType resizeType, int newWidth, params int[] layerIDs)
        {
            return client.ResizeLayer(resizeType, newWidth, layerIDs);
        }

        public Task<bool> ResizeLayer(LayerResizeType resizeType, LayerResizeDirection resizeDirection, double value, params int[] layerIDs)
        {
            return client.ResizeLayer(resizeType, resizeDirection, value, layerIDs);
        }

        public Task<bool> MoveLayer(LayerMoveType moveType, double hPosition, double vPosition, params int[] layerIDs)
        {
            return client.MoveLayer(moveType, hPosition, vPosition, layerIDs);
        }

        public Task<bool> MoveAndResizeLayer(MoveAndResizeType moveType, int hPosition, int vPosition, int hSize, params int[] layerIDs)
        {
            return client.MoveAndResizeLayer(moveType, hPosition, vPosition, hSize, layerIDs);
        }

        #region Treatments

        public Task<bool> LearnTreatment(int treatmentID, int layerID, bool learnPosition, bool learnCrop, bool learnClone, bool learnBorder, bool learnShadow)
        {
            return client.LearnTreatment(treatmentID, layerID, learnPosition, learnCrop, learnClone, learnBorder, learnShadow);
        }

        public Task<bool> AdjustLayerOutsideSoftness(int layerID, int outsideSoftnessThickness)
        {
            return client.AdjustLayerOutsideSoftness(layerID, outsideSoftnessThickness);
        }

        public Task<bool> AdjustLayerBorderBevel(int hBevel, int vBevel, params int[] layerIDs)
        {
            return client.AdjustLayerBorderBevel(hBevel, vBevel, layerIDs);
        }

        public Task<bool> AdjustLayerBorderColor(Color borderColor, params int[] layerIDs)
        {
            return client.AdjustLayerBorderColor(borderColor, layerIDs);
        }

        public Task<bool> AdjustLayerBorder(int layerID, int borderThickness)
        {
            return client.AdjustLayerBorder(layerID, borderThickness);
        }

        public Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor)
        {
            return client.AdjustLayerBorder(layerID, borderThickness, borderColor);
        }

        public Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel)
        {
            return client.AdjustLayerBorder(layerID, borderThickness, borderColor, hBevel, vBevel);
        }

        public Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel, int insideSoftness)
        {
            return client.AdjustLayerBorder(layerID, borderThickness, borderColor, hBevel, vBevel, insideSoftness);
        }

        public Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition)
        {
            return client.AdjustLayerShadow(layerID, hPosition, vPosition);
        }

        public Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size)
        {
            return client.AdjustLayerShadow(layerID, hPosition, vPosition, size);
        }

        public Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency)
        {
            return client.AdjustLayerShadow(layerID, hPosition, vPosition, size, transparency);
        }

        public Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency, int outsideSoftness)
        {
            return client.AdjustLayerShadow(layerID, hPosition, vPosition, size, transparency, outsideSoftness);
        }

        public Task<bool> AdjustLayerCrop(double? leftCrop, double? rightCrop, double? topCrop, double? bottomCrop, params int[] layerIDs)
        {
            return client.AdjustLayerCrop(leftCrop, rightCrop, topCrop, bottomCrop, layerIDs);
        }

        public Task<bool> ResetLayerCrop(params int[] layerIDs)
        {
            return client.ResetLayerCrop(layerIDs);
        }

        public Task<bool> AdjustLayerZoomPan(int layerID, AdjustmentType type, double zoom, int horizontalPan, int verticalPan)
        {
            return client.AdjustLayerZoomPan(layerID, type, zoom, horizontalPan, verticalPan);
        }

        public Task<bool> ResetLayerZoomPan(int layerID)
        {
            return client.ResetLayerZoomPan(layerID);
        }

        public Task<bool> AdjustLayerAspectRatio(AspectRatioAdjustmentType type, double aspectRatioValue, params int[] layerIDs)
        {
            return client.AdjustLayerAspectRatio(type, aspectRatioValue, layerIDs);
        }

        public Task<bool> LayerAssignPixelSpace(int pixelSpaceID, bool makeLayerVisible, params int[] layerIDs)
        {
            return client.LayerAssignPixelSpace(pixelSpaceID, makeLayerVisible, layerIDs);
        }

        #endregion

        #endregion

        #region Input Configuration

        public Task<List<InputPropertyValue>> InputConfigPropertiesGet(int layerID)
        {
            return client.InputConfigPropertiesGet(layerID);
        }

        public Task<bool> InputConfigPropertySet(int layerID, string propertyName, object value)
        {
            return client.InputConfigPropertySet(layerID, propertyName, value);
        }

        public Task<bool> InputConfigPropertySet(int layerID, Dictionary<string, object> propertiesAndValues)
        {
            return client.InputConfigPropertySet(layerID, propertiesAndValues);
        }

        #endregion

        #region Output Configuration

        public Task<List<OutputPropertyValue>> OutputConfigPropertiesGet(int outputIndex)
        {
            return client.OutputConfigPropertiesGet(outputIndex);
        }

        public Task<bool> OutputConfigPropertySet(int outputIndex, string propertyName, object value)
        {
            return client.OutputConfigPropertySet(outputIndex, propertyName, value);
        }

        public Task<bool> OutputConfigPropertySet(int outputIndex, Dictionary<string, object> propertiesAndValues)
        {
            return client.OutputConfigPropertySet(outputIndex, propertiesAndValues);
        }


        public Task<bool> FreezeOutput(params int[] outputIDs)
        {
            return client.FreezeOutput(outputIDs);
        }

        public Task<bool> UnFreezeOutput(params int[] outputIDs)
        {
            return client.UnFreezeOutput(outputIDs);
        }

        public Task<bool> LoadStillOnOutput(string fileName, int outputID, int? dx4ChannelIndex)
        {
            return client.LoadStillOnOutput(fileName, outputID, dx4ChannelIndex);
        }

        public Task<bool> ClearStillOnOutput(int outputID, int? dx4ChannelIndex)
        {
            return client.ClearStillOnOutput(outputID, dx4ChannelIndex);
        }

        public Task<bool> SaveOutputConfiguration(int outputID)
        {
            return client.SaveOutputConfiguration(outputID);
        }

        public Task<bool> SetOutputBlend(int outputID, BlendEdge edge, bool enabled, int blendSize, BlendMode blendMode, float curve1, float curve2)
        {
            return SetOutputBlend(outputID, edge, enabled, blendSize, blendMode, curve1, curve2);
        }

        public Task<bool> ClearOutputBlend(int outputID, BlendEdge edge)
        {
            return client.ClearOutputBlend(outputID, edge);
        }

        public Task<bool> RotateOutput(int outputID, RotationMode mode)
        {
            return client.RotateOutput(outputID, mode);
        }

        public Task<bool> SetOutputModeToNormal(int outputID, int hStart, int vStart, int? dx4ChannelIndex)
        {
            return client.SetOutputModeToNormal(outputID, hStart, vStart, dx4ChannelIndex);
        }

        public Task<bool> SetOutputModeToOpMon(int outputID, int pixelSpaceID)
        {
            return client.SetOutputModeToOpMon(outputID, pixelSpaceID);
        }

        public Task<bool> SetOutputModeToScaled(int outputID, int pixelSpaceID)
        {
            return client.SetOutputModeToScaled(outputID, pixelSpaceID);
        }

        public Task<bool> SetOutputModeToSourceMon(int outputID)
        {
            return client.SetOutputModeToSourceMon(outputID);
        }

        public Task<bool> SetOutputFormat(int outputID, int hActive, int vActive, float refreshRate, bool interlaced, bool useReducedBlanking)
        {
            return client.SetOutputFormat(outputID, hActive, vActive, refreshRate, interlaced, useReducedBlanking);
        }

        #endregion
        
        #region PixelSpace Interaction

        public Task<bool> MixBackground(TimeSpan duration)
        {
            return client.MixBackground(duration);
        }

        public Task<bool> LoadBackgroundImage(int pixelSpaceID, BackgroundImageBus bus, string fileName)
        {
            return client.LoadBackgroundImage(pixelSpaceID, bus, fileName);
        }

        #endregion

        #region Test Pattern Control

        public Task<bool> ClearTestPatternOnPixelSpace(int pixelSpaceID)
        {
            return client.ClearTestPatternOnPixelSpace(pixelSpaceID);
        }
        public Task<bool> ClearTestPatternOnLayer(int layerID)
        {
            return client.ClearTestPatternOnLayer(layerID);
        }
        public Task<bool> ClearTestPatternOnOutput(int outputIndex)
        {
            return client.ClearTestPatternOnOutput(outputIndex);
        }
        public Task<bool> LoadTestPatternToPixelSpace(int pixelSpaceID, TestPatternSettings settings)
        {
            return client.LoadTestPatternToPixelSpace(pixelSpaceID, settings);
        }
        public Task<bool> LoadTestPatternToLayer(int layerID, TestPatternSettings settings)
        {
            return client.LoadTestPatternToLayer(layerID, settings);
        }
        public Task<bool> LoadTestPatternToOutput(int outputIndex, TestPatternSettings settings)
        {
            return client.LoadTestPatternToOutput(outputIndex, settings);
        }

        #endregion

        #region Image Capture

        public Task<Stream> CaptureImageFromOutput(int outputIndex, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            return client.CaptureImageFromOutput(outputIndex, format, maxWidthOrHeight);
        }
        public Task<bool> CaptureImageFromOutput(int outputIndex, Stream targetStream, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            return client.CaptureImageFromOutput(outputIndex, targetStream, format, maxWidthOrHeight);
        }
        public Task<Stream> CaptureImageFromLayer(int layerID, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            return client.CaptureImageFromLayer(layerID, format, maxWidthOrHeight);
        }
        public Task<bool> CaptureImageFromLayer(int layerID, Stream targetStream, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            return client.CaptureImageFromLayer(layerID, targetStream, format, maxWidthOrHeight);
        }
        public Task<Stream> CaptureImageFromInput(int inputIndex, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            return client.CaptureImageFromInput(inputIndex, format, maxWidthOrHeight);
        }
        public Task<bool> CaptureImageFromInput(int inputIndex, Stream targetStream, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            return client.CaptureImageFromInput(inputIndex, targetStream, format, maxWidthOrHeight);
        }

        #endregion

        public Task<bool> SlideLayoutRecall(int pixelSpaceID, bool clearLayers, List<int> reservedLayers, List<SlideLayoutEntry> slideEntries)
        {
            return client.SlideLayoutRecall(pixelSpaceID, clearLayers, reservedLayers, slideEntries);
        }

        public int CompareTo(object obj)
        {
            var compareTo = obj as BindableSpyderClient;
            if (compareTo == null || string.IsNullOrEmpty(this.ServerIP))
                return -1;
            else
                return this.ServerIP.CompareTo(compareTo.ServerIP);
        }
    }
}
