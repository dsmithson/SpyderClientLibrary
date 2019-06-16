using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Net.Notifications;
using Spyder.Client.Scripting;
using Spyder.Client.FunctionKeys;
using Knightware.Primitives;
using Knightware.Diagnostics;
using Spyder.Client.Net.DrawingData;

namespace Spyder.Client.Net
{
    public class SpyderDemoClient : ISpyderClientExtended
    {
        private SpyderDemoServer server;
        private SystemData data;
        public HardwareType HardwareType { get; } = HardwareType.SpyderX80;

        /// <summary>
        /// Contains folder and file paths on the server hardware
        /// </summary>
        public ServerFilePaths ServerFilePaths => ServerFilePaths.FromHardwareType(this.HardwareType, this.Version);

        private string serverIP = "Demo";
        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        public VersionInfo Version => new VersionInfo(0, 0, 0);

        private string hostName;
        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }

        /// <summary>
        /// Defines a throttle for maximum drawing data event raising (per Spyder server).  Setting to 1 second, for example, will ensure DrawingData does not fire more than once per second.  Set to TimeSpan.Zero (default) to disable throttling.
        /// </summary>
        public TimeSpan DrawingDataThrottleInterval { get; set; } = TimeSpan.Zero;

        public event DrawingDataReceivedHandler DrawingDataReceived;
        protected void OnDrawingDataReceived(DrawingDataReceivedEventArgs e)
        {
            if (DrawingDataReceived != null)
                DrawingDataReceived(this, e);
        }

        public event TraceLogMessageHandler TraceLogMessageReceived;
        public event DataObjectChangedHandler DataObjectChanged;
        protected void OnDataObjectChanged(DataObjectChangedEventArgs e)
        {
            if(DataObjectChanged != null)
                DataObjectChanged(this, e);
        }

        protected void OnTraceLogMessageReceived(TraceLogMessageEventArgs e)
        {
            if (TraceLogMessageReceived != null)
                TraceLogMessageReceived(this, e);
        }

        public SpyderDemoClient(SystemData data, HardwareType hardwareType = HardwareType.SpyderX80)
        {
            this.data = data;
            this.HardwareType = hardwareType;
        }

        public async Task<bool> StartupAsync()
        {
            await ShutdownAsync();
            IsRunning = true;

            OnTraceLogMessageReceived(new TraceLogMessageEventArgs(ServerIP, new TraceMessage() { Sender = this, Message = "Starting Demo Server/Client", LogTime = DateTime.Now, Level = TracingLevel.Information }));

            server = new SpyderDemoServer(data);
            server.DrawingDataReceived += server_DrawingDataReceived;

            return true;
        }

        public Task ShutdownAsync()
        {
            IsRunning = false;

            if (server != null)
            {
                OnTraceLogMessageReceived(new TraceLogMessageEventArgs(ServerIP, new TraceMessage() { Sender = this, Message = "Shutting down Demo Server", LogTime = DateTime.Now, Level = TracingLevel.Information }));

                server.DrawingDataReceived -= server_DrawingDataReceived;
                server = null;
            }

            return Task.FromResult(true);
        }

        void server_DrawingDataReceived(object sender, DrawingDataReceivedEventArgs e)
        {
            OnDrawingDataReceived(e);
        }

        public bool IsRunning
        {
            get;
            private set;
        }

        public Task<bool> Save()
        {
            return Task.FromResult(true);
        }

        public Task<VersionInfo> GetVersionInfo()
        {
            return Task.FromResult(server.GetVersionInfo());
        }
        
        public Task<Stream> GetImageFileStream(string fileName, int? maxWidthOrHeight = 2048)
        {
            return Task.FromResult<Stream>(null);
        }

        public Task<bool> GetImageFileStream(string fileName, Stream targetStream, int? maxWidthOrHeight = 2048)
        {
            return Task.FromResult(false);
        }

        public Task<bool> SetImageFileStream(string fileName, Stream fileStream)
        {
            return Task.FromResult(true);
        }

        public Task<List<RegisterPage>> GetRegisterPages(RegisterType type)
        {
            return Task.FromResult(server.GetRegisterPages(type));
        }

        public Task<List<IRegister>> GetRegisters(RegisterType type)
        {
            return Task.FromResult<List<IRegister>>(server.GetRegisters(type).ToList());
        }
        public Task<IRegister> GetRegister(RegisterType type, int registerID)
        {
            return Task.FromResult(server.GetRegister(type, registerID));
        }

        public Task<List<Source>> GetSources()
        {
            return Task.FromResult<List<Source>>(server.GetRegisters(RegisterType.Source).Cast<Source>().ToList());
        }
        public Task<Source> GetSource(int sourceRegisterID)
        {
            return Task.FromResult(server.GetListItem<Source>(RegisterType.Source, sourceRegisterID));
        }
        public Task<Source> GetSource(IRegister sourceRegister)
        {
            return Task.FromResult(server.GetListItem<Source>(sourceRegister));
        }

        public Task<Source> GetSource(string sourceName)
        {
            return Task.FromResult(server.GetSource(sourceName));
        }

        public Task<List<CommandKey>> GetCommandKeys()
        {
            return Task.FromResult<List<CommandKey>>(server.GetRegisters(RegisterType.CommandKey).Cast<CommandKey>().ToList());
        }
        public Task<CommandKey> GetCommandKey(int commandKeyRegisterID)
        {
            return Task.FromResult(server.GetListItem<CommandKey>(RegisterType.CommandKey, commandKeyRegisterID));
        }
        public Task<CommandKey> GetCommandKey(IRegister commandKeyRegister)
        {
            return Task.FromResult(server.GetListItem<CommandKey>(commandKeyRegister));
        }

        public Task<Script> GetScript(int scriptID)
        {
            return Task.FromResult(server.GetScript(scriptID));
        }

        public Task<bool> DeleteCommandKey(params int[] commandKeyRegisterIDs)
        {
            return Task.FromResult(server.DeleteCommandKey(commandKeyRegisterIDs));
        }

        public Task<int> GetRunningCommandKeyCue(int registerID)
        {
            return Task.FromResult(-1);
        }

        public Task<List<Script>> GetScripts()
        {
            return Task.FromResult<List<Script>>(server.GetScripts());
        }

        public Task<List<Treatment>> GetTreatments()
        {
            return Task.FromResult<List<Treatment>>(server.GetRegisters(RegisterType.Treatment).Cast<Treatment>().ToList());
        }
        public Task<Treatment> GetTreatment(int treatmentRegisterID)
        {
            return Task.FromResult<Treatment>(server.GetListItem<Treatment>(RegisterType.Treatment, treatmentRegisterID));
        }
        public Task<Treatment> GetTreatment(IRegister treatmentRegister)
        {
            return Task.FromResult<Treatment>(server.GetListItem<Treatment>(treatmentRegister));
        }

        public Task<List<FunctionKey>> GetFunctionKeys()
        {
            return Task.FromResult<List<FunctionKey>>(server.GetRegisters(RegisterType.FunctionKey).Cast<FunctionKey>().ToList());
        }
        public Task<FunctionKey> GetFunctionKey(int functionKeyRegisterID)
        {
            return Task.FromResult(server.GetListItem<FunctionKey>(RegisterType.FunctionKey, functionKeyRegisterID));
        }
        public Task<FunctionKey> GetFunctionKey(IRegister functionKeyRegister)
        {
            return Task.FromResult(server.GetListItem<FunctionKey>(functionKeyRegister));
        }

        public Task<List<Still>> GetStills()
        {
            return Task.FromResult<List<Still>>(server.GetRegisters(RegisterType.Still).Cast<Still>().ToList());
        }
        public Task<Still> GetStill(int stillRegisterID)
        {
            return Task.FromResult(server.GetListItem<Still>(RegisterType.Still, stillRegisterID));
        }
        public Task<Still> GetStill(IRegister stillRegister)
        {
            return Task.FromResult(server.GetListItem<Still>(stillRegister));
        }

        public Task<List<PlayItem>> GetPlayItems()
        {
            return Task.FromResult<List<PlayItem>>(server.GetRegisters(RegisterType.PlayItem).Cast<PlayItem>().ToList());
        }
        public Task<PlayItem> GetPlayItem(int playItemRegisterID)
        {
            return Task.FromResult(server.GetListItem<PlayItem>(RegisterType.PlayItem, playItemRegisterID));
        }
        public Task<PlayItem> GetPlayItem(IRegister playItemRegister)
        {
            return Task.FromResult(server.GetListItem<PlayItem>(playItemRegister));
        }

        public Task<List<PixelSpace>> GetPixelSpaces()
        {
            return Task.FromResult(server.GetPixelSpaces());
        }

        public Task<PixelSpace> GetPixelSpace(int pixelSpaceID)
        {
            return Task.FromResult(server.GetPixelSpace(pixelSpaceID));
        }

        public Task<List<InputConfig>> GetInputConfigs()
        {
            return Task.FromResult(server.GetInputConfigs());
        }

        public Task<InputConfig> GetInputConfig(int inputConfigID)
        {
            return Task.FromResult(server.GetInputConfig(inputConfigID));
        }

        public Task<InputConfig> GetInputConfig(string sourceName)
        {
            return Task.FromResult(server.GetInputConfig(sourceName));
        }

        public Task<bool> LearnCommandKey(int pageIndex, int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative)
        {
            return Task.FromResult(server.LearnCommandKey(pageIndex, registerID, name, learnFrom, learnAsMixers, learnAsRelative));
        }

        public Task<bool> LearnCommandKey(int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative)
        {
            return Task.FromResult(server.LearnCommandKey(registerID, name, learnFrom, learnAsMixers, learnAsRelative));
        }
        public Task<bool> RecallCommandKey(int registerID, int cueIndex)
        {
            return Task.FromResult(server.RecallCommandKey(registerID, cueIndex));
        }
        public Task<bool> RecallFunctionKey(int registerID)
        {
            return Task.FromResult(server.RecallFunctionKey(registerID));
        }
        public Task<bool> RecallRegisterToLayer(RegisterType registerType, int registerID, params int[] layerIDs)
        {
            return Task.FromResult(server.RecallRegisterToLayer(registerType, registerID, layerIDs));
        }

        public Task<List<Shape>> GetShapes()
        {
            return Task.FromResult(server.GetShapes());
        }

        public Task<Shape> GetShape(string shapeFileName)
        {
            return Task.FromResult(server.GetShape(shapeFileName));
        }

        public Task<bool> SetShape(Shape shape)
        {
            return Task.FromResult(server.SetShape(shape));
        }

        public Task<List<string>> GetShapeFileNames()
        {
            return Task.FromResult(server.GetShapeFileNames());
        }

        public Task<ServerSettings> GetServerSettings()
        {
            return Task.FromResult(server.GetServerSettings());
        }

        #region Layer Interaction

        public Task<List<KeyframePropertyValue>> KeyframePropertiesGet(int layerID)
        {
            return Task.FromResult(new List<KeyframePropertyValue>());
        }

        public Task<bool> KeyframePropertiesSet(int layerID, string propertyName, object value)
        {
            return Task.FromResult(true);
        }

        public Task<bool> KeyframePropertiesSet(int layerID, Dictionary<string, object> propertiesAndValues)
        {
            return Task.FromResult(true);
        }

        public Task<int> GetLayerCount()
        {
            return Task.FromResult(server.RequestLayerCount());
        }

        public Task<int> GetFirstAvailableLayerID()
        {
            return Task.FromResult(server.GetFirstAvailableLayer());
        }

        public Task<bool> ApplyRegisterToLayer(RegisterType type, int registerID, params int[] layerIDs)
        {
            return Task.FromResult(server.ApplyRegisterToLayer(type, registerID, layerIDs));
        }

        public Task<bool> FreezeLayer(params int[] layerIDs)
        {
            return Task.FromResult(server.FreezeLayer(layerIDs));
        }

        public Task<bool> UnFreezeLayer(params int[] layerIDs)
        {
            return Task.FromResult(server.UnFreezeLayer(layerIDs));
        }

        public async Task<bool> MixOnLayer(int pixelSpaceID, Point position, int width, TimeSpan duration, Register content)
        {
            return await MixOnLayer(pixelSpaceID, position, width, await GetDurationInFrames(duration), content);
        }

        public Task<bool> MixOnLayer(int pixelSpaceID, Point position, int width, int duration, Register content)
        {
            return Task.FromResult(server.MixOnLayer(pixelSpaceID, position, width, duration, content));
        }

        public async Task<bool> MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, TimeSpan duration, Register content)
        {
            return await MixOnLayer(layerID, pixelSpaceID, position, width, await GetDurationInFrames(duration), content);
        }

        public Task<bool> MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, int duration, Register content)
        {
            return Task.FromResult(server.MixOnLayer(layerID, pixelSpaceID, position, width, duration, content));
        }

        public async Task<bool> MixOffAllLayers(TimeSpan duration)
        {
            return await MixOffAllLayers(await GetDurationInFrames(duration));
        }

        public Task<bool> MixOffAllLayers(int duration)
        {
            return Task.FromResult(server.MixOffAllLayers(duration));
        }

        public async Task<bool> MixOffLayer(TimeSpan duration, params int[] layerIDs)
        {
            return await MixOffLayer(await GetDurationInFrames(duration), layerIDs);
        }

        public Task<bool> MixOffLayer(int duration, params int[] layerIDs)
        {
            return Task.FromResult(server.MixOffLayer(duration, layerIDs));
        }

        public async Task<bool> MixOnLayer(TimeSpan duration, params int[] layerIDs)
        {
            return await MixOnLayer(await GetDurationInFrames(duration), layerIDs);
        }

        public Task<bool> MixOnLayer(int duration, params int[] layerIDs)
        {
            return Task.FromResult(server.MixOnLayer(duration, layerIDs));
        }

        public async Task<bool> TransitionLayer(TimeSpan duration, params int[] layerIDs)
        {
            return await TransitionLayer(await GetDurationInFrames(duration), layerIDs);
        }

        public Task<bool> TransitionLayer(int duration, params int[] layerIDs)
        {
            return Task.FromResult(server.TransitionLayer(duration, layerIDs));
        }

        public Task<bool> ResizeLayer(LayerResizeType type, int newWidth, params int[] layerIDs)
        {
            return Task.FromResult(server.ResizeLayer(type, newWidth, layerIDs));
        }

        public Task<bool> ResizeLayer(LayerResizeType resizeType, LayerResizeDirection resizeDirection, double value, params int[] layerIDs)
        {
            return Task.FromResult(server.ResizeLayer(resizeType, resizeDirection, value, layerIDs));
        }

        public Task<bool> MoveLayer(LayerMoveType moveType, double hPosition, double vPosition, params int[] layerIDs)
        {
            return Task.FromResult(server.MoveLayer(moveType, hPosition, vPosition, layerIDs));
        }

        public Task<bool> MoveAndResizeLayer(MoveAndResizeType moveType, int hPosition, int vPosition, int hSize, params int[] layerIDs)
        {
            return Task.FromResult(server.MoveAndResizeLayer(moveType, hPosition, vPosition, hSize, layerIDs));
        }

        #region Treatments

        public Task<bool> LearnTreatment(int treatmentID, int layerID, bool learnPosition, bool learnCrop, bool learnClone, bool learnBorder, bool learnShadow)
        {
            return Task.FromResult(server.LearnTreatment(treatmentID, layerID, learnPosition, learnCrop, learnClone, learnBorder, learnShadow));
        }

        public Task<bool> AdjustLayerOutsideSoftness(int layerID, int outsideSoftnessThickness)
        {
            return Task.FromResult(server.AdjustLayerOutsideSoftness(layerID, outsideSoftnessThickness));
        }

        public Task<bool> AdjustLayerBorderBevel(int hBevel, int vBevel, params int[] layerIDs)
        {
            return Task.FromResult(server.AdjustLayerBorderBevel(hBevel, vBevel, layerIDs));
        }

        public Task<bool> AdjustLayerBorderColor(Color borderColor, params int[] layerIDs)
        {
            return Task.FromResult(server.AdjustLayerBorderColor(borderColor, layerIDs));
        }

        public Task<bool> AdjustLayerBorder(int layerID, int borderThickness)
        {
            return Task.FromResult(server.AdjustLayerBorder(layerID, borderThickness));
        }

        public Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor)
        {
            return Task.FromResult(server.AdjustLayerBorder(layerID, borderThickness, borderColor));
        }

        public Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel)
        {
            return Task.FromResult(server.AdjustLayerBorder(layerID, borderThickness, borderColor, hBevel, vBevel));
        }

        public Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel, int insideSoftness)
        {
            return Task.FromResult(server.AdjustLayerBorder(layerID, borderThickness, borderColor, hBevel, vBevel, insideSoftness));
        }

        public Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition)
        {
            return Task.FromResult(server.AdjustLayerShadow(layerID, hPosition, vPosition));
        }

        public Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size)
        {
            return Task.FromResult(server.AdjustLayerShadow(layerID, hPosition, vPosition, size));
        }

        public Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency)
        {
            return Task.FromResult(server.AdjustLayerShadow(layerID, hPosition, vPosition, size, transparency));
        }

        public Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency, int outsideSoftness)
        {
            return Task.FromResult(server.AdjustLayerShadow(layerID, hPosition, vPosition, size, transparency, outsideSoftness));
        }

        public Task<bool> AdjustLayerCrop(double? leftCrop, double? rightCrop, double? topCrop, double? bottomCrop, params int[] layerIDs)
        {
            return Task.FromResult(server.AdjustLayerCrop(leftCrop, rightCrop, topCrop, bottomCrop, layerIDs));
        }

        public Task<bool> ResetLayerCrop(params int[] layerIDs)
        {
            return Task.FromResult(server.AdjustLayerCrop(0, 0, 0, 0, layerIDs));
        }

        public Task<bool> AdjustLayerZoomPan(int layerID, AdjustmentType type, double zoom, int horizontalPan, int verticalPan)
        {
            return Task.FromResult(server.AdjustZoomPan(layerID, type, zoom, horizontalPan, verticalPan));
        }

        public Task<bool> ResetLayerZoomPan(int layerID)
        {
            return Task.FromResult(server.AdjustZoomPan(layerID, AdjustmentType.Absolute, 0, 0, 0));
        }

        public Task<bool> AdjustLayerAspectRatio(AspectRatioAdjustmentType type, double aspectRatioValue, params int[] layerIDs)
        {
            return Task.FromResult(server.AdjustLayerAspectRatio(type, aspectRatioValue, layerIDs));
        }

        public Task<bool> LayerAssignPixelSpace(int pixelSpaceID, bool makeLayerVisible, params int[] layerIDs)
        {
            return Task.FromResult(server.LayerAssignPixelSpace(pixelSpaceID, makeLayerVisible, layerIDs));
        }

        #endregion

        #endregion

        #region Input Configuration

        public Task<List<InputPropertyValue>> InputConfigPropertiesGet(int layerID)
        {
            return Task.FromResult(new List<InputPropertyValue>());
        }

        public Task<bool> InputConfigPropertiesSet(int layerID, string propertyName, object value)
        {
            return Task.FromResult(true);
        }

        public Task<bool> InputConfigPropertiesSet(int layerID, Dictionary<string, object> propertiesAndValues)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region Output Configuration

        public Task<List<OutputPropertyValue>> OutputConfigPropertiesGet(int outputIndex)
        {
            return Task.FromResult(new List<OutputPropertyValue>());
        }
        public Task<bool> OutputConfigPropertiesSet(int outputIndex, string propertyName, object value)
        {
            return Task.FromResult(true);
        }

        public Task<bool> OutputConfigPropertiesSet(int outputIndex, Dictionary<string, object> propertiesAndValues)
        {
            return Task.FromResult(true);
        }

        public Task<bool> FreezeOutput(params int[] outputIDs)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UnFreezeOutput(params int[] outputIDs)
        {
            return Task.FromResult(true);
        }

        public Task<bool> LoadStillOnOutput(string fileName, int outputID, int? dx4ChannelIndex)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ClearStillOnOutput(int outputID, int? dx4ChannelIndex)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SaveOutputConfiguration(int outputID)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetOutputBlend(int outputID, BlendEdge edge, bool enabled, int blendSize, BlendMode blendMode, float curve1, float curve2)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ClearOutputBlend(int outputID, BlendEdge edge)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RotateOutput(int outputID, RotationMode mode)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetOutputModeToNormal(int outputID, int hStart, int vStart, int? dx4ChannelIndex)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetOutputModeToOpMon(int outputID, int pixelSpaceID)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetOutputModeToScaled(int outputID, int pixelSpaceID)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetOutputModeToSourceMon(int outputID)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetOutputFormat(int outputID, int hActive, int vActive, float refreshRate, bool interlaced, bool useReducedBlanking)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region PixelSpace Interaction

        public Task<bool> MixBackground(int duration)
        {
            return Task.FromResult(server.MixBackground(duration));
        }

        public async Task<bool> MixBackground(TimeSpan duration)
        {
            return await MixBackground(await GetDurationInFrames(duration));
        }

        public Task<bool> LoadBackgroundImage(int pixelSpaceID, BackgroundImageBus bus, string fileName)
        {
            return server.LoadBackgroundImage(pixelSpaceID, bus, fileName);
        }

        #endregion

        #region Test Pattern Control

        public Task<bool> ClearTestPatternOnPixelSpace(int pixelSpaceID)
        {
            return Task.FromResult(true);

        }
        public Task<bool> ClearTestPatternOnLayer(int layerID)
        {
            return Task.FromResult(true);
        }
        public Task<bool> ClearTestPatternOnOutput(int outputIndex)
        {
            return Task.FromResult(true);
        }
        public Task<bool> LoadTestPatternToPixelSpace(int pixelSpaceID, TestPatternSettings settings)
        {
            return Task.FromResult(true);
        }
        public Task<bool> LoadTestPatternToLayer(int layerID, TestPatternSettings settings)
        {
            return Task.FromResult(true);
        }
        public Task<bool> LoadTestPatternToOutput(int outputIndex, TestPatternSettings settings)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region Image Capture

        public Task<Stream> CaptureImageFromOutput(int outputIndex, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            int size = maxWidthOrHeight ?? 1024;
            return Task.FromResult(Knightware.Drawing.BitmapHelper.GenerateSolidColorBitmap(Colors.Blue, size, (int)(size / 1.777f)));
        }
        public Task<bool> CaptureImageFromOutput(int outputIndex, Stream targetStream, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            int size = maxWidthOrHeight ?? 1024;
            Knightware.Drawing.BitmapHelper.GenerateSolidColorBitmap(targetStream, Colors.Blue, size, (int)(size / 1.777f));
            return Task.FromResult(true);
        }
        public Task<Stream> CaptureImageFromLayer(int layerID, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            int size = maxWidthOrHeight ?? 1024;
            return Task.FromResult(Knightware.Drawing.BitmapHelper.GenerateSolidColorBitmap(Colors.Blue, size, (int)(size / 1.777f)));
        }
        public Task<bool> CaptureImageFromLayer(int layerID, Stream targetStream, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            int size = maxWidthOrHeight ?? 1024;
            Knightware.Drawing.BitmapHelper.GenerateSolidColorBitmap(targetStream, Colors.Blue, size, (int)(size / 1.777f));
            return Task.FromResult(true);
        }
        public Task<Stream> CaptureImageFromInput(int inputIndex, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            int size = maxWidthOrHeight ?? 1024;
            return Task.FromResult(Knightware.Drawing.BitmapHelper.GenerateSolidColorBitmap(Colors.Blue, size, (int)(size / 1.777f)));
        }
        public Task<bool> CaptureImageFromInput(int inputIndex, Stream targetStream, ImageFileFormat format = ImageFileFormat.Bmp, int? maxWidthOrHeight = null)
        {
            int size = maxWidthOrHeight ?? 1024;
            Knightware.Drawing.BitmapHelper.GenerateSolidColorBitmap(targetStream, Colors.Blue, size, (int)(size / 1.777f));
            return Task.FromResult(true);
        }

        #endregion

        public Task<bool> SlideLayoutRecall(int pixelSpaceID, bool clearLayers, List<int> reservedLayers, List<SlideLayoutEntry> slideEntries)
        {
            return Task.FromResult(false);
        }


        private async Task<int> GetDurationInFrames(TimeSpan duration)
        {
            var serverSettings = await GetServerSettings();
            int framesPerSecond = (serverSettings == null ? 60 : TimeCode.FramesPerSecond(serverSettings.FieldRate));
            int frames = (int)(framesPerSecond * duration.TotalSeconds);
            return frames;
        }
    }
}
