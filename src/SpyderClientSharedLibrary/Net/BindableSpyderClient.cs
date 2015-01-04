using Spyder.Client.Common;
using Spyder.Client.Diagnostics;
using Spyder.Client.Threading;
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
using Spyder.Client.Primitives;

namespace Spyder.Client.Net
{
    /// <summary>
    /// Wraps a SpyderClient instance and provides additional bindable collections from a Spyder server
    /// </summary>
    public class BindableSpyderClient : DispatcherPropertyChangedBase, ISpyderClientExtended, IComparable
    {
        private const int maxTraceMessages = 100;
        private readonly ISpyderClientExtended client;

        #region Properties

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

        private ObservableCollection<TraceMessage> traceMessages = new ObservableCollection<TraceMessage>();
        public ObservableCollection<TraceMessage> TraceMessages
        {
            get { return traceMessages; }
            set
            {
                if (traceMessages != value)
                {
                    traceMessages = value;
                    OnPropertyChanged();
                }
            }
        }

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
            if (commandKeys != null)
                await GetCommandKeys();

            if (scripts != null)
                await GetScripts();

            if (functionKeys != null)
                await GetFunctionKeys();

            if (sources != null)
                await GetSources();

            if (treatments != null)
                await GetTreatments();

            if (stills != null)
                await GetStills();

            if (playItems != null)
                await GetPlayItems();

            if (pixelSpaces != null)
                await GetPixelSpaces();

            if (inputConfigs != null)
                await GetInputConfigs();

            return true;
        }

        #region ISpyderClient interface wrapper

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

        public async Task<bool> Startup()
        {
            Shutdown();

            //Hook for trace messages so we can see messages related to connections from the server side
            client.TraceLogMessageReceived += client_TraceLogMessageReceived;

            if (!await client.Startup())
            {
                Shutdown();
                return false;
            }

            //Do additional initialization here
            client.DrawingDataReceived += client_DrawingDataReceived;
            
            return true;
        }

        
        public void Shutdown()
        {
            client.DrawingDataReceived -= client_DrawingDataReceived;
            client.TraceLogMessageReceived -= client_TraceLogMessageReceived;

            client.Shutdown();
        }

        void client_DrawingDataReceived(object sender, DrawingDataReceivedEventArgs e)
        {
            if (!IsRunning)
                return;

            try
            {
                //Update overall status
                if(e.DrawingData == null)
                {
                    this.UserDiagnosticOverallStatus = DiagnosticStatus.Unknown;
                }
                else if(e.DrawingData.DiagnosticWarnings == null || e.DrawingData.DiagnosticWarnings.Count == 0)
                {
                    this.userDiagnosticOverallStatus = DiagnosticStatus.NotRun;
                }
                else
                {
                    this.UserDiagnosticOverallStatus = e.DrawingData.DiagnosticWarnings.Values.Min();
                }

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
                if(updateSeverity)
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

        public Task<Stream> GetImageFileStream(string fileName)
        {
            return client.GetImageFileStream(fileName);
        }

        public Task<IEnumerable<RegisterPage>> GetRegisterPages(RegisterType type)
        {
            return client.GetRegisterPages(type);
        }
        public Task<IEnumerable<IRegister>> GetRegisters(RegisterType type)
        {
            return client.GetRegisters(type);
        }
        public Task<IRegister> GetRegister(RegisterType type, int registerID)
        {
            return client.GetRegister(type, registerID);
        }

        public Task<IEnumerable<Source>> GetSources()
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
        
        public Task<IEnumerable<CommandKey>> GetCommandKeys()
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

        public Task<IEnumerable<Script>> GetScripts()
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

        public Task<IEnumerable<Treatment>> GetTreatments()
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

        public Task<IEnumerable<FunctionKey>> GetFunctionKeys()
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

        public Task<IEnumerable<Still>> GetStills()
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

        public Task<IEnumerable<PlayItem>> GetPlayItems()
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

        public Task<IEnumerable<PixelSpace>> GetPixelSpaces()
        {
            var task = client.GetPixelSpaces();
            task.ContinueWith((t) => UpdateList(t, (list) => this.PixelSpaces = list));
            return task;
        }

        public Task<PixelSpace> GetPixelSpace(int pixelSpaceID)
        {
            return client.GetPixelSpace(pixelSpaceID);
        }

        public Task<IEnumerable<InputConfig>> GetInputConfigs()
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
        private void UpdateList<T>(Task<IEnumerable<T>> newCollectionTask, Action<ObservableCollection<T>> updateMethod)
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

        public Task<int> RequestLayerCount()
        {
            return client.RequestLayerCount();
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

        #region PixelSpace Interaction

        public Task<bool> MixBackground(TimeSpan duration)
        {
            return client.MixBackground(duration);
        }

        public Task<bool> LoadBackgroundImage(int pixelSpaceID, BackgroundImageBus bus, string fileName)
        {
            return client.LoadBackgroundImage(pixelSpaceID, bus, fileName);
        }

        public Task<List<PixelSpace>> RequestPixelSpaces()
        {
            return client.RequestPixelSpaces();
        }

        public Task<PixelSpace> RequestPixelSpace(int pixelSpaceID)
        {
            return client.RequestPixelSpace(pixelSpaceID);
        }

        #endregion

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
