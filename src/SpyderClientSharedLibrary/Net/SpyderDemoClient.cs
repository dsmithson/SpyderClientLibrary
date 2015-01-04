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
using Spyder.Client.Primitives;
using Spyder.Client.Diagnostics;

namespace Spyder.Client.Net
{
    public class SpyderDemoClient : ISpyderClientExtended
    {
        private SpyderDemoServer server;
        private SystemData data;

        private string serverIP = "Demo";
        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        private string hostName;
        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
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

        public SpyderDemoClient(SystemData data)
        {
            this.data = data;
        }

        public Task<bool> Startup()
        {
            Shutdown();
            IsRunning = true;

            OnTraceLogMessageReceived(new TraceLogMessageEventArgs(ServerIP, new TraceMessage() { Sender = this, Message = "Starting Demo Server/Client", LogTime = DateTime.Now, Level = TracingLevel.Information }));

            server = new SpyderDemoServer(data);
            server.DrawingDataReceived += server_DrawingDataReceived;

            return Task.FromResult(true);
        }

        public void Shutdown()
        {
            IsRunning = false;

            if (server != null)
            {
                OnTraceLogMessageReceived(new TraceLogMessageEventArgs(ServerIP, new TraceMessage() { Sender = this, Message = "Shutting down Demo Server", LogTime = DateTime.Now, Level = TracingLevel.Information }));

                server.DrawingDataReceived -= server_DrawingDataReceived;
                server = null;
            }
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
        
        public Task<Stream> GetImageFileStream(string fileName)
        {
            return Task.FromResult<Stream>(null);
        }

        public Task<IEnumerable<RegisterPage>> GetRegisterPages(RegisterType type)
        {
            return Task.FromResult(server.GetRegisterPages(type));
        }

        public Task<IEnumerable<IRegister>> GetRegisters(RegisterType type)
        {
            return Task.FromResult<IEnumerable<IRegister>>(server.GetRegisters(type).ToList());
        }
        public Task<IRegister> GetRegister(RegisterType type, int registerID)
        {
            return Task.FromResult(server.GetRegister(type, registerID));
        }

        public Task<IEnumerable<Source>> GetSources()
        {
            return Task.FromResult<IEnumerable<Source>>(server.GetRegisters(RegisterType.Source).Cast<Source>());
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

        public Task<IEnumerable<CommandKey>> GetCommandKeys()
        {
            return Task.FromResult<IEnumerable<CommandKey>>(server.GetRegisters(RegisterType.CommandKey).Cast<CommandKey>());
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

        public Task<IEnumerable<Script>> GetScripts()
        {
            return Task.FromResult<IEnumerable<Script>>(server.GetScripts());
        }

        public Task<IEnumerable<Treatment>> GetTreatments()
        {
            return Task.FromResult<IEnumerable<Treatment>>(server.GetRegisters(RegisterType.Treatment).Cast<Treatment>());
        }
        public Task<Treatment> GetTreatment(int treatmentRegisterID)
        {
            return Task.FromResult<Treatment>(server.GetListItem<Treatment>(RegisterType.Treatment, treatmentRegisterID));
        }
        public Task<Treatment> GetTreatment(IRegister treatmentRegister)
        {
            return Task.FromResult<Treatment>(server.GetListItem<Treatment>(treatmentRegister));
        }

        public Task<IEnumerable<FunctionKey>> GetFunctionKeys()
        {
            return Task.FromResult<IEnumerable<FunctionKey>>(server.GetRegisters(RegisterType.FunctionKey).Cast<FunctionKey>());
        }
        public Task<FunctionKey> GetFunctionKey(int functionKeyRegisterID)
        {
            return Task.FromResult(server.GetListItem<FunctionKey>(RegisterType.FunctionKey, functionKeyRegisterID));
        }
        public Task<FunctionKey> GetFunctionKey(IRegister functionKeyRegister)
        {
            return Task.FromResult(server.GetListItem<FunctionKey>(functionKeyRegister));
        }

        public Task<IEnumerable<Still>> GetStills()
        {
            return Task.FromResult<IEnumerable<Still>>(server.GetRegisters(RegisterType.Still).Cast<Still>());
        }
        public Task<Still> GetStill(int stillRegisterID)
        {
            return Task.FromResult(server.GetListItem<Still>(RegisterType.Still, stillRegisterID));
        }
        public Task<Still> GetStill(IRegister stillRegister)
        {
            return Task.FromResult(server.GetListItem<Still>(stillRegister));
        }

        public Task<IEnumerable<PlayItem>> GetPlayItems()
        {
            return Task.FromResult<IEnumerable<PlayItem>>(server.GetRegisters(RegisterType.PlayItem).Cast<PlayItem>());
        }
        public Task<PlayItem> GetPlayItem(int playItemRegisterID)
        {
            return Task.FromResult(server.GetListItem<PlayItem>(RegisterType.PlayItem, playItemRegisterID));
        }
        public Task<PlayItem> GetPlayItem(IRegister playItemRegister)
        {
            return Task.FromResult(server.GetListItem<PlayItem>(playItemRegister));
        }

        public Task<IEnumerable<PixelSpace>> GetPixelSpaces()
        {
            return Task.FromResult(server.GetPixelSpaces());
        }

        public Task<PixelSpace> GetPixelSpace(int pixelSpaceID)
        {
            return Task.FromResult(server.GetPixelSpace(pixelSpaceID));
        }

        public Task<IEnumerable<InputConfig>> GetInputConfigs()
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

        public Task<List<string>> GetShapeFileNames()
        {
            return Task.FromResult(server.GetShapeFileNames());
        }

        public Task<ServerSettings> GetServerSettings()
        {
            return Task.FromResult(server.GetServerSettings());
        }
        
        #region Layer Interaction

        public Task<int> RequestLayerCount()
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

        public Task<PixelSpace> RequestPixelSpace(int pixelSpaceID)
        {
            return Task.FromResult(server.RequestPixelSpace(pixelSpaceID));
        }

        public Task<List<PixelSpace>> RequestPixelSpaces()
        {
            return Task.FromResult(server.RequestPixelSpaces());
        }

        #endregion

        private async Task<int> GetDurationInFrames(TimeSpan duration)
        {
            var serverSettings = await GetServerSettings();
            int framesPerSecond = (serverSettings == null ? 60 : TimeCode.FramesPerSecond(serverSettings.FieldRate));
            int frames = (int)(framesPerSecond * duration.TotalSeconds);
            return frames;
        }
    }
}
