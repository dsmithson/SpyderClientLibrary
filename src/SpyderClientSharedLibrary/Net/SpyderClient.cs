using Spyder.Client.Common;
using Knightware.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Net.Notifications;
using Spyder.Client.Scripting;
using Spyder.Client.FunctionKeys;
using PCLStorage;
using Knightware.Primitives;
using Knightware.Net.Sockets;
using Spyder.Client.Net.DrawingData;

namespace Spyder.Client.Net
{
    /// <summary>
    /// Enhances UDP version of Spyder client to provide more detailed system data
    /// </summary>
    public class SpyderClient : SpyderUdpClient, ISpyderClientExtended
    {
        private readonly Func<IStreamSocket> getStreamSocket;
        private readonly Func<Task<SpyderServerEventListenerBase>> getServerEventListener;

        private SpyderServerEventListenerBase serverEventListener;
        private IFolder localCacheFolder;
        private QFTClient qftClient;
        private SystemData systemData;
        private DrawingData.DrawingData drawingData;
        private int lastDataObjectVersion = -1;

        /// <summary>
        /// Defines a throttle for maximum drawing data event raising (per Spyder server).  Setting to 1 second, for example, will ensure DrawingData does not fire more than once per second.  Set to TimeSpan.Zero (default) to disable throttling.
        /// </summary>
        public TimeSpan DrawingDataThrottleInterval
        {
            get { return drawingDataThrottleInterval; }
            set
            {
                drawingDataThrottleInterval = value;
                serverEventListener.DrawingDataThrottleInterval = value;
            }
        }
        private TimeSpan drawingDataThrottleInterval = TimeSpan.Zero;

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

        public VersionInfo Version { get; set; }

        public string ServerName { get; set; }
        
        public SpyderClient(Func<Task<SpyderServerEventListenerBase>> getServerEventListener, Func<IStreamSocket> getStreamSocket, Func<IUDPSocket> getUdpSocket, string serverIP, IFolder localCacheFolder)
            : base(getUdpSocket, serverIP)
        {
            this.getServerEventListener = getServerEventListener;
            this.getStreamSocket = getStreamSocket;
            this.localCacheFolder = localCacheFolder;
        }

        public override async Task<bool> StartupAsync()
        {
            if (!await base.StartupAsync())
                return false;
            
            //Init our QFT Client
            qftClient = new QFTClient(getStreamSocket, ServerIP);
            if (!await qftClient.StartupAsync())
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Failed to startup QFT Client.  Shutting down...");
                await ShutdownAsync();
                return false;
            }

            if (!await LoadDataAsync())
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Failed to load system data.  Shutting down...");
                await ShutdownAsync();
                return false;
            }

            //Startup event listener
            serverEventListener = await getServerEventListener();
            if (serverEventListener != null)
            {
                serverEventListener.DrawingDataReceived += serverEventListener_DrawingDataReceived;
                serverEventListener.ServerAnnounceMessageReceived += serverEventListener_ServerAnnounceMessageReceived;
                serverEventListener.TraceLogMessageReceived += serverEventListener_TraceLogMessageReceived;
            }

            return true;
        }

        public override async Task ShutdownAsync()
        {
            //Stop event listener
            if (serverEventListener != null)
            {
                serverEventListener.DrawingDataReceived -= serverEventListener_DrawingDataReceived;
                serverEventListener.ServerAnnounceMessageReceived -= serverEventListener_ServerAnnounceMessageReceived;
                serverEventListener.TraceLogMessageReceived -= serverEventListener_TraceLogMessageReceived;
                serverEventListener = null;
            }

            //Stop QFT client
            if (qftClient != null)
            {
                await qftClient.ShutdownAsync();
                qftClient = null;
            }

            drawingData = null;
            lastDataObjectVersion = -1;

            await base.ShutdownAsync();
        }
        
        private void serverEventListener_DrawingDataReceived(object sender, DrawingDataReceivedEventArgs e)
        {
            if (IsRunning && e.ServerIP == this.ServerIP)
            {
                drawingData = e.DrawingData;
                OnDrawingDataReceived(e);

                //Check dataobject changed.  It's a good check here even though we get an explicit dataObjectChanged message
                //from the server, because it's possible that we'll miss the UDP message
                if (e.DrawingData != null && lastDataObjectVersion != e.DrawingData.DataObjectVersion)
                {
                    int dataObjectVersion = e.DrawingData.DataObjectVersion;
                    DataType changeType = e.DrawingData.DataObjectLastChangeType;
                    if (dataObjectVersion - lastDataObjectVersion != 1 && lastDataObjectVersion != -1)
                        changeType = DataType.All;

                    lastDataObjectVersion = dataObjectVersion;
                    OnDataObjectChanged(new DataObjectChangedEventArgs(ServerIP, dataObjectVersion, changeType));
                }
            }
        }

        private void serverEventListener_ServerAnnounceMessageReceived(object sender, SpyderServerAnnounceInformation serverInfo)
        {
            if (serverInfo != null && serverInfo.Address == this.ServerIP)
            {
                //Update our version info
                if (this.Version == null || !this.Version.Equals(serverInfo.Version))
                    this.Version = serverInfo.Version;
            }
        }

        void serverEventListener_TraceLogMessageReceived(object sender, TraceLogMessageEventArgs e)
        {
            if(e != null && e.Message != null && e.Address == this.ServerIP)
            {
                OnTraceLogMessageReceived(e);
            }
        }

        public Task<VersionInfo> GetVersionInfo()
        {
            return Task.FromResult(this.Version);
        }

        public async Task<bool> LoadDataAsync()
        {
            if (!IsRunning || qftClient == null || !qftClient.IsRunning)
                return false;

            //Try to force a server save before continuing
            if (!await Save())
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Failed to save data at the server before loading.  Data being loaded may be stale");
            }

            //TODO:  Save this data into isolated storage and possibly even load from there by default
            MemoryStream configFile = null;
            MemoryStream scriptFile = null;
            try
            {
                //Download system configuration file
                configFile = new MemoryStream();
                string configFileName = qftClient.ConvertAbsolutePathToRelative(@"c:\Spyder\SystemConfiguration.xml");
                if (!await qftClient.ReceiveFile(configFileName, configFile, null))
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Failed to download SystemConfiguration.xml from server");
                    return false;
                }

                //Download script file
                scriptFile = new MemoryStream();
                string scriptFileName = qftClient.ConvertAbsolutePathToRelative(@"c:\Spyder\Scripts\Scripts.xml");
                if (!await qftClient.ReceiveFile(scriptFileName, scriptFile, null))
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Failed to download Scripts.xml from server");
                    return false;
                }

                //Load system data
                scriptFile.Seek(0, SeekOrigin.Begin);
                configFile.Seek(0, SeekOrigin.Begin);

                if (systemData == null)
                {
                    systemData = new SystemData();
                }

                if (!await systemData.LoadDataAsync(configFile, scriptFile))
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Failed to load system data");
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while downloading system data files: {1}", ex.GetType().Name, ex.Message);
                return false;
            }
            finally
            {
                if (configFile != null)
                {
                    configFile.Dispose();
                    configFile = null;
                }
                if (scriptFile != null)
                {
                    scriptFile.Dispose();
                    scriptFile = null;
                }
            }
        }

        public override Task<List<RegisterPage>> GetRegisterPages(RegisterType type)
        {
            switch (type)
            {
                case RegisterType.CommandKey:
                    return Task.FromResult<List<RegisterPage>>(new List<RegisterPage>(systemData.CommandKeyPages));
                case RegisterType.FunctionKey:
                    return Task.FromResult<List<RegisterPage>>(new List<RegisterPage>(systemData.FunctionKeyPages));
                default:
                    return base.GetRegisterPages(type);
            }
        }

        public override Task<List<IRegister>> GetRegisters(RegisterType type)
        {
            switch (type)
            {
                case RegisterType.PlayItem:
                    return Task.FromResult<List<IRegister>>(new List<IRegister>(systemData.PlayItems));
                case RegisterType.CommandKey:
                    return Task.FromResult<List<IRegister>>(new List<IRegister>(systemData.CommandKeys));
                case RegisterType.Treatment:
                    return Task.FromResult<List<IRegister>>(new List<IRegister>(systemData.Treatments));
                case RegisterType.Source:
                    return Task.FromResult<List<IRegister>>(new List<IRegister>(systemData.Sources));
                case RegisterType.FunctionKey:
                    return Task.FromResult<List<IRegister>>(new List<IRegister>(systemData.FunctionKeys));
                //case RegisterType.Still:
                //    //TODO:  Wire this up
                //    break;
                default:
                    return base.GetRegisters(type);
            }
        }

        public override Task<CommandKey> GetCommandKey(int commandKeyRegisterID)
        {
            return GetItemFromListByRegister(systemData.CommandKeys, commandKeyRegisterID);
        }

        public override Task<CommandKey> GetCommandKey(IRegister commandKeyRegister)
        {
            return GetItemFromListByRegister(systemData.CommandKeys, commandKeyRegister);
        }

        public override Task<List<CommandKey>> GetCommandKeys()
        {
            return GetItemListFromSystemData(systemData.CommandKeys);
        }

        public virtual Task<List<Script>> GetScripts()
        {
            if (systemData == null || systemData.Scripts == null)
                return Task.FromResult<List<Script>>(null);

            return Task.FromResult((List<Script>)systemData.Scripts);
        }
        public virtual Task<Script> GetScript(int scriptID)
        {
            if (systemData == null || systemData.Scripts == null)
                return Task.FromResult<Script>(null);

            return Task.FromResult(systemData.Scripts.FirstOrDefault(s => s.ID == scriptID));
        }

        public virtual Task<List<InputConfig>> GetInputConfigs()
        {
            if (systemData == null || systemData.InputConfigs == null)
                return Task.FromResult<List<InputConfig>>(null);

            return Task.FromResult((List<InputConfig>)systemData.InputConfigs);
        }
        public virtual Task<InputConfig> GetInputConfig(int InputConfigID)
        {
            if (systemData == null || systemData.InputConfigs == null)
                return Task.FromResult<InputConfig>(null);

            return Task.FromResult(systemData.InputConfigs.FirstOrDefault(s => s.ID == InputConfigID));
        }

        public virtual async Task<InputConfig> GetInputConfig(string sourceName)
        {
            Source source = await GetSource(sourceName);
            return (source == null ? null : await GetInputConfig(source.InputConfigID));
        }

        public override Task<List<PixelSpace>> GetPixelSpaces()
        {
            if (systemData == null || systemData.PixelSpaces == null)
                return Task.FromResult<List<PixelSpace>>(null);

            return Task.FromResult((List<PixelSpace>)systemData.PixelSpaces);
        }
        public override Task<PixelSpace> GetPixelSpace(int PixelSpaceID)
        {
            if (systemData == null || systemData.PixelSpaces == null)
                return Task.FromResult<PixelSpace>(null);

            return Task.FromResult(systemData.PixelSpaces.FirstOrDefault(s => s.ID == PixelSpaceID));
        }

        public override Task<FunctionKey> GetFunctionKey(int functionKeyRegisterID)
        {
            return GetItemFromListByRegister(systemData.FunctionKeys, functionKeyRegisterID);
        }

        public override Task<FunctionKey> GetFunctionKey(IRegister functionKeyRegister)
        {
            return GetItemFromListByRegister(systemData.FunctionKeys, functionKeyRegister);
        }

        public override Task<List<FunctionKey>> GetFunctionKeys()
        {
            return GetItemListFromSystemData(systemData.FunctionKeys);
        }

        public override Task<PlayItem> GetPlayItem(int playItemRegisterID)
        {
            return GetItemFromListByRegister(systemData.PlayItems, playItemRegisterID);
        }

        public override Task<PlayItem> GetPlayItem(IRegister playItemRegister)
        {
            return GetItemFromListByRegister(systemData.PlayItems, playItemRegister);
        }

        public override Task<List<PlayItem>> GetPlayItems()
        {
            return GetItemListFromSystemData(systemData.PlayItems);
        }

        public override Task<Source> GetSource(int sourceRegisterID)
        {
            return GetItemFromListByRegister(systemData.Sources, sourceRegisterID);
        }

        public override Task<Source> GetSource(IRegister sourceRegister)
        {
            return GetItemFromListByRegister(systemData.Sources, sourceRegister);
        }

        public override Task<Source> GetSource(string sourceName)
        {
            if (systemData != null && systemData.Sources != null && !string.IsNullOrEmpty(sourceName))
            {
                return Task.FromResult(systemData.Sources.FirstOrDefault(s => s.Name == sourceName));
            }
            return Task.FromResult<Source>(null);
        }

        public override Task<List<Source>> GetSources()
        {
            return GetItemListFromSystemData(systemData.Sources);
        }

        public override Task<Still> GetStill(int stillRegisterID)
        {
            return GetItemFromListByRegister(systemData.Stills, stillRegisterID);
        }

        public override Task<Still> GetStill(IRegister stillRegister)
        {
            return GetItemFromListByRegister(systemData.Stills, stillRegister);
        }

        public override Task<List<Still>> GetStills()
        {
            return GetItemListFromSystemData(systemData.Stills);
        }

        public override Task<Treatment> GetTreatment(int treatmentRegisterID)
        {
            return GetItemFromListByRegister(systemData.Treatments, treatmentRegisterID);
        }

        public override Task<Treatment> GetTreatment(IRegister treatmentRegister)
        {
            return GetItemFromListByRegister(systemData.Treatments, treatmentRegister);
        }

        public override Task<List<Treatment>> GetTreatments()
        {
            return GetItemListFromSystemData(systemData.Treatments);
        }
        
        public override Task<IRegister> GetRegister(RegisterType type, int registerID)
        {
            IEnumerable<IRegister> list = null;
            switch (type)
            {
                case RegisterType.PlayItem:
                    list = systemData.PlayItems;
                    break;
                case RegisterType.CommandKey:
                    list = systemData.CommandKeys;
                    break;
                case RegisterType.Treatment:
                    list = systemData.Treatments;
                    break;
                case RegisterType.Source:
                    list = systemData.Sources;
                    break;
                case RegisterType.FunctionKey:
                    list = systemData.FunctionKeys;
                    break;
                case RegisterType.Still:
                    list = systemData.Stills;
                    break;
                default:
                    throw new NotSupportedException(string.Format("List item type of {0} is not supported", type));
            }

            if (list == null)
                return null;

            return GetItemFromListByRegister(list, registerID);
        }

        #region Wrappers for SystemData list access

        private Task<T> GetItemFromListByRegister<T>(IEnumerable<T> systemDataList, int registerID) where T: IRegister
        {
            if (systemDataList == null)
                return Task.FromResult<T>(default(T));

            return Task.FromResult(systemDataList.FirstOrDefault(listItem => listItem.RegisterID == registerID));
        }

        private Task<T> GetItemFromListByRegister<T>(List<T> systemDataList, IRegister register) where T : IRegister
        {
            if (register == null || systemDataList == null)
                return Task.FromResult<T>(default(T));

            return Task.FromResult(systemDataList.FirstOrDefault(listItem => listItem.RegisterID == register.RegisterID));
        }

        private Task<List<T>> GetItemListFromSystemData<T>(List<T> systemDataList)
        {
            return Task.FromResult(systemDataList);
        }

        #endregion

        public override async Task<Stream> GetImageFileStream(string fileName)
        {
            if (!IsRunning || qftClient == null || !qftClient.IsRunning || string.IsNullOrEmpty(fileName))
                return null;

            try
            {
                //First lets look in our cache for the file
                if (localCacheFolder != null)
                {
                    IFolder imageCacheFolder = await localCacheFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                    IFile file = (await imageCacheFolder.GetFilesAsync()).FirstOrDefault(f => string.Compare(f.Name, fileName, StringComparison.CurrentCultureIgnoreCase) == 0);
                    if (file != null)
                    {
                        return await file.OpenAsync(FileAccess.Read);
                    }
                }

                //Not found in cache; try to get file from server
                MemoryStream response = new MemoryStream();
                string absolutePath = Path.Combine(@"c:\spyder\images", Path.GetFileName(fileName));
                string relativePath = qftClient.ConvertAbsolutePathToRelative(absolutePath);
                if (await qftClient.ReceiveFile(relativePath, response, null))
                {
                    //Success.  Save file to local cache
                    if (localCacheFolder != null)
                    {
                        IFolder imageCacheFolder = await localCacheFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                        IFile file = await imageCacheFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                        using (Stream fileStream = await file.OpenAsync(FileAccess.ReadAndWrite))
                        {
                            response.Seek(0, SeekOrigin.Begin);
                            await response.CopyToAsync(fileStream);
                        }
                    }

                    //Rewind our memory stream and return it now
                    response.Seek(0, SeekOrigin.Begin);
                    return response;
                }
                else
                {
                    //Failed to acquire image.  Cleanup and let the base class try
                    response.Dispose();
                    response = null;
                    return await base.GetImageFileStream(fileName);
                }
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while getting image file {1} from server: {2}", ex.GetType().Name, fileName, ex.Message);
                return null;
            }
        }

        public Task<bool> SetImageFileStream(string fileName, Stream fileStream)
        {
            return  SetImageFileStream(fileName, fileStream, true);
        }

        public async Task<bool> SetImageFileStream(string fileName, Stream fileStream, bool writeFileToLocalCache)
        {
            if (!IsRunning || qftClient == null || !qftClient.IsRunning || string.IsNullOrEmpty(fileName) || fileStream == null)
                return false;

            try
            {
                if(fileStream.Position != 0)
                    fileStream.Seek(0, SeekOrigin.Begin);

                //Write file to the server
                string absolutePath = Path.Combine(@"c:\spyder\images", Path.GetFileName(fileName));
                string relativePath = qftClient.ConvertAbsolutePathToRelative(absolutePath);
                if (!await qftClient.SendFile(fileStream, relativePath, null))
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Failed to send image file {0} to Spyder server", fileName);
                    return false;
                }

                //Write file to our local cache
                if (writeFileToLocalCache && localCacheFolder != null)
                {
                    IFolder imageCacheFolder = await localCacheFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
                    IFile file = await imageCacheFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    using (Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite))
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                        await fileStream.CopyToAsync(stream);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while sending file {1} to server: {2}", ex.GetType().Name, fileName, ex.Message);
                return false;
            }
        }

        public async Task<List<string>> GetShapeFileNames()
        {
            var files = await GetImageFileNames();
            if (files == null)
                return null;

            return new List<string>(files
                .Where(file => file.ToLower().EndsWith(".shape"))
                .Select(file => Path.GetFileName(file)));
        }

        public async Task<List<string>> GetImageFileNames()
        {
            if (!IsRunning || qftClient == null || !qftClient.IsRunning)
                return null;

            string absolutePath = @"c:\spyder\images";
            string relativePath = qftClient.ConvertAbsolutePathToRelative(absolutePath);
            var files = await qftClient.GetFiles(relativePath);
            if (files == null || files.Length == 0)
                return new List<string>(0);
            else
                return new List<string>(files);
        }

        public async Task<List<Shape>> GetShapes()
        {
            List<string> files = await GetImageFileNames();
            if (files == null)
                return null;

            List<Shape> response = new List<Shape>();
            foreach (string file in files.Where(f => string.Compare(Path.GetExtension(f), ".shape", StringComparison.CurrentCultureIgnoreCase) == 0))
            {
                Shape shape = await GetShape(Path.GetFileName(file));
                if (shape != null)
                {
                    response.Add(shape);
                }
                else
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Failed to obtain or parse shape file for {0}", file);
                }
            }
            return response;
        }

        public async Task<Shape> GetShape(string shapeFileName)
        {
            if (string.IsNullOrEmpty(shapeFileName))
                return null;

            Stream shapeStream = null;
            try
            {
                shapeStream = await GetImageFileStream(shapeFileName);
                if (shapeStream != null)
                {
                    Shape response = Shape.Parse(shapeFileName, shapeStream);
                    return response;
                }
                return null;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while getting shape file: {1}", ex.GetType().Name, ex.Message);
                return null;
            }
            finally
            {
                if (shapeStream != null)
                {
                    shapeStream.Dispose();
                    shapeStream = null;
                }
            }
        }

        public async Task<bool> SetShape(Shape shape)
        {
            if (shape != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    if (await shape.ToShapeFileStream(stream))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        return await SetImageFileStream(shape.Name, stream);
                    }
                }
            }
            return false;
        }

        public async Task<ServerSettings> GetServerSettings()
        {
            if (!IsRunning || qftClient == null || !qftClient.IsRunning)
                return null;

            VersionInfo version = await GetVersionInfo();
            if (version == null)
                return null;

            //For debug purposes, we connect to local remoting servers.  We'll check here if the first check fails
            List<string> settingsFileLocations = new List<string>()
            {
                //Versions prior to (4.0.6) keep the SystemSettings.xml file with the server executable
                @"c:\Applications\SpyderServer\Version {0}.{1}.{2}\SystemSettings.xml",

                //Newer versions (4.0.6 and later) keep the SystemSettings.xml in a global path
                @"c:\Spyder\SystemSettings.xml",
                
#if DEBUG
                //64-bit machines
                @"c:\Program Files (x86)\Vista Systems, Corp\Spyder Control Suite 2012 {0}.{1}.{2}\Version {0}.{1}.{2}\SystemSettings.xml",
                @"c:\Program Files (x86)\Vista Systems, Corp\Spyder Control Suite 2009 {0}.{1}.{2}\Version {0}.{1}.{2}\SystemSettings.xml",
                @"c:\Program Files (x86)\Vista Systems, Corp\Spyder Control Suite 2005 {0}.{1}.{2}\Version {0}.{1}.{2}\SystemSettings.xml",
                @"c:\Program Files (x86)\Vista Systems, Corp\Vista Advanced\Version {0}.{1}.{2}\SystemSettings.xml",

                //32-bit Machines
                @"c:\Program Files\Vista Systems, Corp\Spyder Control Suite 2012 {0}.{1}.{2}\Version {0}.{1}.{2}\SystemSettings.xml",
                @"c:\Program Files\Vista Systems, Corp\Spyder Control Suite 2009 {0}.{1}.{2}\Version {0}.{1}.{2}\SystemSettings.xml",
                @"c:\Program Files\Vista Systems, Corp\Spyder Control Suite 2005 {0}.{1}.{2}\Version {0}.{1}.{2}\SystemSettings.xml",
                @"c:\Program Files\Vista Systems, Corp\Vista Advanced\Version {0}.{1}.{2}\SystemSettings.xml"
#endif
            };

            foreach (string settingsFile in settingsFileLocations)
            {
                try
                {
                    //Format our file name
                    string qftFileName = QFTClient.ConvertToRelativePath(string.Format(settingsFile, version.Major, version.Minor, version.Build));

                    if (await qftClient.FileExists(qftFileName))
                    {
                        using (var stream = new MemoryStream())
                        {
                            if (await qftClient.ReceiveFile(qftFileName, stream, null))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                ServerSettings response = new ServerSettings();
                                if (response.Load(stream))
                                {
                                    return response;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to get system settings file: {1}", ex.GetType().Name, ex.Message);
                }
            }

            //Not found
            return null;
        }
        
        #region PixelSpace Interactions

        public async Task<bool> MixBackground(TimeSpan duration)
        {
            //Let base client send the UDP transition with frames
            return await MixBackground(await GetDurationInFrames(duration));
        }

        #endregion

        #region Layer Interactions

        public async Task<bool> MixOffAllLayers(TimeSpan duration)
        {
            return await MixOffAllLayers(await GetDurationInFrames(duration));
        }

        public async Task<bool> MixOffLayer(TimeSpan duration, params int[] layerIDs)
        {
            return await MixOffLayer(await GetDurationInFrames(duration), layerIDs);
        }

        public async Task<bool> MixOnLayer(TimeSpan duration, params int[] layerIDs)
        {
            return await MixOnLayer(await GetDurationInFrames(duration), layerIDs);
        }

        public async Task<bool> MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, TimeSpan duration, Register content)
        {
            return await MixOnLayer(layerID, pixelSpaceID, position, width, await GetDurationInFrames(duration), content);
        }

        public async Task<bool> MixOnLayer(int pixelSpaceID, Point position, int width, TimeSpan duration, Register content)
        {
            return await MixOnLayer(pixelSpaceID, position, width, await GetDurationInFrames(duration), content);
        }

        public async Task<bool> TransitionLayer(TimeSpan duration, params int[] layerIDs)
        {
            return await TransitionLayer(await GetDurationInFrames(duration), layerIDs);
        }

        protected override Task<LayerKeyFrameInfo> RequestLayerKeyFrame(int layerID)
        {
            var data = this.drawingData;
            var dkf = (data == null ? null : data.GetLayer(layerID, MixerBus.Program));
            if(dkf == null)
            {
                return base.RequestLayerKeyFrame(layerID);
            }
            else
            {
                //Fill a layer keyframe object to return
                var response = new LayerKeyFrameInfo(dkf);
                return Task.FromResult(response);
            }
        }

        #endregion

        private async Task<int> GetDurationInFrames(TimeSpan duration)
        {
            int frames = 60;
            try
            {
                var settings = await GetServerSettings();
                if (settings != null)
                {
                    //Derive frame count from seconds and the system's internal frame rate
                    frames = (int)(duration.TotalSeconds * TimeCode.FramesPerSecond(settings.FieldRate));
                }
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning,
                    "{0} occurred while trying to get system frame rate for background mix.  Defaulting to 60 frame transition.  Message: {1}",
                    ex.GetType().Name, ex.Message);
            }

            //Quick sanity check
            if (frames < 1)
                frames = 1;

            return frames;
        }

        protected override Task<List<PixelSpaceMapping>> RequestPixelSpaceMappings()
        {
            var drawingData = this.drawingData;
            if (drawingData == null)
            {
                return base.RequestPixelSpaceMappings();
            }
            else
            {
                //Build a response object using our existing drawing data instance
                var response = new List<PixelSpaceMapping>();
                foreach(var mapping in drawingData.PreviewPixelSpaceIDs)
                {
                    var pgm = drawingData.GetPixelSpace(mapping.Key);
                    var pvw = drawingData.GetPixelSpace(mapping.Value);
                    if(pgm != null && pvw != null)
                    {
                        response.Add(new PixelSpaceMapping(pgm.ID, pvw.ID, pvw.Scale));
                    }
                }
                return Task.FromResult(response);
            }
        }
    }
}
