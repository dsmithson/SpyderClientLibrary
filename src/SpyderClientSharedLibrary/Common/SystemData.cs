using Knightware.Diagnostics;
using Knightware.Net;
using Knightware.Primitives;
using Spyder.Client.Scripting;
using Knightware.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spyder.Client.FunctionKeys;
using System.Xml;
using Spyder.Client.IO;

namespace Spyder.Client.Common
{
    public class SystemData
    {
        private SpyderXmlDeserializer deserializer = new SpyderXmlDeserializer();

        public List<Treatment> Treatments { get; protected set; }

        public List<PixelSpace> PixelSpaces { get; protected set; }

        public Dictionary<int, int> PreviewPixelSpaces { get; protected set; }

        public List<PlayItem> PlayItems { get; protected set; }

        public List<Source> Sources { get; protected set; }

        public List<InputConfig> InputConfigs { get; protected set; }

        public List<CommandKey> CommandKeys { get; protected set; }

        public List<RegisterPage> CommandKeyPages { get; protected set; }

        public List<FunctionKey> FunctionKeys { get; protected set; }

        public List<RegisterPage> FunctionKeyPages { get; protected set; }

        public List<Still> Stills { get; protected set; }

        public List<Router> Routers { get; protected set; }

        public List<Script> Scripts { get; protected set; }

        public SystemData()
        {
            Treatments = new List<Treatment>();
            PixelSpaces = new List<PixelSpace>();
            PlayItems = new List<PlayItem>();
            Sources = new List<Source>();
            InputConfigs = new List<InputConfig>();
            CommandKeys = new List<CommandKey>();
            CommandKeyPages = new List<RegisterPage>();
            FunctionKeys = new List<FunctionKey>();
            FunctionKeyPages = new List<RegisterPage>();
            Stills = new List<Still>();
            Routers = new List<Router>();
            Scripts = new List<Script>();
        }

        public Task<bool> LoadDataAsync(Stream systemConfigurationFile, Stream scriptsFile)
        {
            return Task.Run(() => LoadData(systemConfigurationFile, scriptsFile));
        }

        protected bool LoadData(Stream systemConfigurationFile, Stream scriptsFile)
        {
            ClearData();
            try
            {
                //Get XDocument instances
                var configDocument = deserializer.GetXDocument(systemConfigurationFile);
                var scriptDocument = deserializer.GetXDocument(scriptsFile);
                if (configDocument == null)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Configuration XML missing.  Unable to parse system configuration.");
                    return false;
                }
                if (scriptDocument == null)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Script XML missing.  Unable to parse system configuration.");
                    return false;
                }

                //Deserialize our data lists
                var pixelSpaces = ParsePixelSpaces(configDocument);
                var previewPixelSpaces = ParsePreviewPixelSpaces(configDocument);
                var sources = ParseSources(configDocument);
                var treatments = ParseTreatments(configDocument);
                var routers = ParseRouters(configDocument);
                var playItems = ParsePlayItems(configDocument);
                var inputConfigs = ParseInputConfigs(configDocument);
                var stills = ParseStills(configDocument);
                var functionKeys = ParseFunctionKeys(configDocument);
                var functionKeyPages = ParseRegisterPageList(configDocument, RegisterType.FunctionKey);

                var scripts = ParseScripts(scriptDocument);
                var commandKeys = ParseCommandKeys(configDocument, scriptDocument, scripts);
                var commandKeyPages = ParseRegisterPageList(scriptDocument, RegisterType.CommandKey);

                //Set our main data objects
                this.Sources = sources;
                this.PixelSpaces = pixelSpaces;
                this.PreviewPixelSpaces = previewPixelSpaces;
                this.Treatments = treatments;
                this.Routers = routers;
                this.Scripts = scripts;
                this.PlayItems = playItems;
                this.InputConfigs = inputConfigs;
                this.Stills = stills;
                this.CommandKeys = commandKeys;
                this.CommandKeyPages = commandKeyPages;
                this.FunctionKeys = functionKeys;
                this.FunctionKeyPages = functionKeyPages;

                return true;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while loading system file: {1}", ex.GetType().Name, ex.Message);
                ClearData();
                return false;
            }
        }

        public void ClearData()
        {
            this.Sources = null;
            this.PixelSpaces = null;
            this.Treatments = null;
            this.Routers = null;
            this.Scripts = null;
            this.PlayItems = null;
            this.InputConfigs = null;
            this.Stills = null;
        }

        protected List<PixelSpace> ParsePixelSpaces(XDocument document)
        {
            return ParseList(() =>
                {
                    return document.Descendants("PixelSpaces")
                                     .SelectDescendantsByValueResult()
                                     .Select((item) =>
                                     {
                                         string rect = deserializer.Read(item, "Rect", "0,0,0,0");
                                         string[] rectParts = rect.Split(',');

                                         return new PixelSpace()
                                         {
                                             ID = deserializer.Read(item, "ID", -1),
                                             Name = deserializer.Read(item, "Name", string.Empty),
                                             Scale = deserializer.Read(item, "Scale", 1f),
                                             LastBackgroundStill = deserializer.Read(item, "LastBackgroundStill", string.Empty),
                                             NextBackgroundStill = deserializer.Read(item, "NextBackgroundStill", string.Empty),
                                             X = int.Parse(rectParts[0]),
                                             Y = int.Parse(rectParts[1]),
                                             Width = int.Parse(rectParts[2]),
                                             Height = int.Parse(rectParts[3]),
                                             RenewMasterFrameID = deserializer.Read(item, "RenewMasterFrameID", -1),
                                             StereoMode = deserializer.ReadEnum(item, "StereoMode", PixelSpaceStereoMode.Off),
                                             LayerTitleBackground = deserializer.Read(item, "LayerTitleBackground", new Color(0, 0, 0)),
                                             LayerTitleForeground = deserializer.Read(item, "LayerTitleForeground", new Color(0, 0, 0)),
                                             LayerTitleLocation = deserializer.ReadEnum(item, "LayerTitleLocation", TitleLocation.InsideOfLayer),
                                             LayerTitlePosition = deserializer.ReadEnum(item, "LayerTitlePosition", TitlePosition.BottomCenter),
                                             LayerTitleSize = deserializer.ReadEnum(item, "LayerTitleSize", TitleSize.Small)
                                         };
                                     });
                });
        }

        protected Dictionary<int, int> ParsePreviewPixelSpaces(XDocument document)
        {
            var results = ParseList(() =>
            {
                return document.Descendants("PreviewPixelSpaces")
                                 .SelectMany((item) => item.Descendants("Item"))
                                 .Select((item) =>
                                 {
                                     return new Tuple<int, int>(
                                         deserializer.Read(item, "Key", -1),
                                         deserializer.Read(item, "Value", -1));
                                 });
            });

            return results.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        protected List<Source> ParseSources(XDocument document)
        {
            return ParseList(
                document,
                RegisterType.Source,

                //Item processor
                () => document.Descendants("Sources")
                        .SelectDescendantsByValueResult()
                        .Select((item) => new Source()
                        {
                            Name = deserializer.Read(item, "Name", string.Empty),
                            Thumbnail = deserializer.Read(item, "Thumbnail", string.Empty),
                            InputConfigID = deserializer.Read(item, "InputConfigID", -1),
                            RouterID = deserializer.Read(item, "RouterID", -1),
                            RouterInput = deserializer.Read(item, "RouterInput", -1),
                            PreferredLayerID = deserializer.Read(item, "PreferredLayer", -1),
                            PreferredTreatmentID = deserializer.Read(item, "PreferredTreatment", -1),
                            LinearKeySource = deserializer.Read(item, "LinearKeySource", string.Empty),
                        }),

                //Register Item Lookup
                (itemList, register) => itemList.FirstOrDefault(s => string.Compare(s.Name, register.Name, StringComparison.CurrentCultureIgnoreCase) == 0));
        }

        protected List<FunctionKey> ParseFunctionKeys(XDocument document)
        {
            return ParseList(
                document,
                RegisterType.FunctionKey,

                //Item processor
                () => document.Descendants("FunctionKeys")
                        .SelectMany((item) => item.Elements("Item"))
                        .Select((item) =>
                            {
                                FunctionKey functionKey = null;

                                string type = GetAttributeFromTypeAttribute(item);
                                item = item.Element("Value") ?? item;

                                if (type.Contains("AssignSourceKey"))
                                {
                                    functionKey = new AssignSourceFunctionKey()
                                    {
                                        SourceName = item.Element("Source").Value
                                    };
                                }
                                else if (type.Contains("StillLoaderKey"))
                                {
                                    functionKey = new AssignStillFunctionKey()
                                    {
                                        FileName = item.Element("FileName").Value
                                    };
                                }
                                else if (type.Contains("MixBackgroundKey"))
                                {
                                    functionKey = new MixBackgroundFunctionKey()
                                    {
                                        Duration = int.Parse(item.Element("Duration").Value)
                                    };
                                }
                                else if (type.Contains("ScriptRecallKey"))
                                {
                                    functionKey = new ScriptRecallFunctionKey()
                                    {
                                        ScriptID = int.Parse(item.Element("ScriptID").Value),
                                        CueIndex = int.Parse(item.Element("CueID").Value),
                                        ServerIP = item.Element("ServerIPString").Value
                                    };
                                }
                                else if (type.Contains("FreezeLayerKey"))
                                {
                                    functionKey = new FreezeLayerKey()
                                    {
                                        IsFreezeEnabled = bool.Parse(item.Element("Freeze").Value)
                                    };
                                }
                                else if (type.Contains("FreezeDeviceKey"))
                                {
                                    functionKey = new FreezeDeviceKey()
                                    {
                                        IsFreezeEnabled = bool.Parse(item.Element("FreezeEnabled").Value),
                                        SetTopLayer = bool.Parse(item.Element("SetTopLayer").Value),
                                        SetBottomLayer = bool.Parse(item.Element("SetBottomLayer").Value)
                                    };
                                }
                                else if (type.Contains("PositionOffsetKey"))
                                {
                                    functionKey = new LayerPositionOffsetFunctionKey()
                                    {
                                        HOffset = float.Parse(item.Element("HOffset").Value),
                                        VOffset = float.Parse(item.Element("VOffset").Value)
                                    };
                                }
                                else if (type.Contains("NetworkCommandKey"))
                                {
                                    functionKey = new NetworkCommandFunctionKey()
                                    {
                                        Port = int.Parse(item.Element("Port").Value),
                                        Command = item.Element("Command").Value,
                                        ServerAddress = item.Element("ServerAddress").Value,
                                        ProtocolType = (NetworkProtocolType)Enum.Parse(typeof(NetworkProtocolType), item.Element("CommandType").Value)
                                    };
                                }
                                else if (type.Contains("RouterSalvoKey"))
                                {
                                    functionKey = new RouterSalvoFunctionKey()
                                    {
                                        Salvo = ParseRouterSalvo(item.Element("Salvo"))
                                    };
                                }
                                else if (type.Contains("BackupSourcePresetKey"))
                                {
                                    functionKey = new BackupSourceFunctionKey()
                                    {
                                        SourceName = item.Element("SourceName").Value,
                                        SourceLookupID = int.Parse(item.Element("SourceLookupId").Value),
                                        BackupSourceName = item.Element("BackupSourceName").Value,
                                        BackupType = (SourceBackupType)Enum.Parse(typeof(SourceBackupType), item.Element("BackupType").Value)
                                    };
                                }
                                else
                                {
                                    //Default to a generic function key structure
                                    functionKey = new FunctionKey();
                                }

                                //Update all base function key parameters
                                functionKey.LookupID = int.Parse(item.Element("ID").Value);
                                functionKey.Name = item.Element("Name").Value;

                                //Update relative properties (if applicable)
                                var relativeFunctionKey = functionKey as IRelativeFunctionKey;
                                if (relativeFunctionKey != null)
                                {
                                    relativeFunctionKey.IsAbsolute = bool.Parse(item.Element("Absolute").Value);
                                    relativeFunctionKey.AbsoluteLayerIDs = item.Element("AbsoluteLayers")
                                        .Elements("Item").Select(i => int.Parse(i.Value)).ToList();
                                }

                                return functionKey;
                            }),

                //Register Item Lookup
                (itemList, register) => itemList.FirstOrDefault(f => f.LookupID == register.LookupID));
        }

        protected List<CommandKey> ParseCommandKeys(XDocument configDocument, XDocument scriptsDocument, List<Script> scripts)
        {
            List<CommandKey> response = ParseList(
                configDocument,
                RegisterType.CommandKey,

                //Item processor
                () => scriptsDocument.Descendants("CommandKeys")
                    .SelectDescendantsByValueResult()
                    .Select((item) =>
                    {
                        var commandKey = new CommandKey()
                        {
                            LookupID = int.Parse(item.Element("ID").Value),
                            ScriptID = int.Parse(item.Element("ScriptID").Value),
                            Name = item.Element("DisplayText").Value,
                            RegisterID = (int.Parse(item.Element("Page").Value) * 1000 + int.Parse(item.Element("KeyID").Value))
                        };

                        //Spyder studio (5.0+) refers to absolute as IsAbsolute - Advanced (4.x and below) refer to this as Absolute
                        var absoluteNode = item.Element("Absolute");
                        if (absoluteNode != null)
                        {
                            //V4 style
                            commandKey.IsRelative = !bool.Parse(absoluteNode.Value);
                        }
                        else if ((absoluteNode = item.Element("ScriptType")) != null)
                        {
                            //V5 style
                            commandKey.IsRelative = absoluteNode.Value.Contains("Relative");
                            commandKey.RegisterColorDefined = bool.Parse(item.Element("HasCustomColor").Value);
                            commandKey.RegisterColor = Color.FromHexString(item.Element("CustomColor").Value);
                        }
                        else
                        {
                            throw new InvalidDataException("Unable to find relative/absolute status for command key");
                        }

                        return commandKey;
                    }),

                //Register Item Lookup
                (itemList, register) => itemList.FirstOrDefault(c => c.LookupID == register.LookupID && c.RegisterID == register.RegisterID));

            //Flush out our command key properties with their associated scripts
            if (scripts != null)
            {
                int index = 0;
                while (index < response.Count)
                {
                    var cmdKey = response[index];
                    var script = scripts.FirstOrDefault(s => s.ID == cmdKey.ScriptID);

                    if (script != null)
                    {
                        cmdKey.CueCount = script.Cues.Count;
                        cmdKey.IsRelative = (cmdKey.IsRelative || script.IsRelative); //V5 will store isRelative on the cmdKey, V4 will store it on the script
                        script.IsRelative = (cmdKey.IsRelative || script.IsRelative);
                        //cmdKey.Name = script.Name;
                        index++;
                    }
                    else
                    {
                        //Remove command key from response, since it is invalid
                        response.RemoveAt(index);
                    }
                }
            }

            return response;
        }

        protected List<Still> ParseStills(XDocument document)
        {
            return ParseList(() => ParseRegisterList(document, RegisterType.Still).Select(register => new Still(register)));
        }

        protected List<PlayItem> ParsePlayItems(XDocument document)
        {
            return ParseList(
                document,
                RegisterType.PlayItem,

                //Item processor
                () => document.Descendants("PlayItems")
                        .SelectDescendantsByValueResult()
                        .Select((item) => new PlayItem()
                        {
                            ID = deserializer.Read(item, "ID", -1),
                            Name = deserializer.Read(item, "Name", string.Empty),
                            ClipName = deserializer.Read(item, "ClipName", string.Empty),
                            DefaultSource = deserializer.Read(item, "DefaultSource", string.Empty),
                            Duration = deserializer.Read(item, "Duration", new TimeCode()),
                            InTime = deserializer.Read(item, "InTime", new TimeCode()),
                            OutTime = deserializer.Read(item, "OutTime", new TimeCode()),
                            FieldRate = deserializer.ReadEnum(item, "FPS", FieldRate.FR_29_97),
                            PreRollFrames = deserializer.Read(item, "PrerollFrames", 0),
                            RollOutFrames = deserializer.Read(item, "RolloutFrames", 0),
                            MachineFlags = (byte)deserializer.Read(item, "MachineFlags", 0),
                            Thumbnail = deserializer.Read(item, "Thumbnail", string.Empty),
                        }),

                //Register Item Lookup
                (itemList, register) => itemList.FirstOrDefault(s => string.Compare(s.Name, register.Name, StringComparison.CurrentCultureIgnoreCase) == 0));
        }

        protected List<Treatment> ParseTreatments(XDocument document)
        {
            return ParseList(
                document,
                RegisterType.Treatment,

                //Item Processor
                () => document.Descendants("Treatments")
                    .SelectDescendantsByValueResult()
                    .Select((item) => new Treatment(ParseKeyFrame(item))
                    {
                        ID = deserializer.Read(item, "ID", -1),
                        IsDurationEnabled = deserializer.Read(item, "DurationActive", false),
                        IsHPositionEnabled = deserializer.Read(item, "HPosActive", false),
                        IsVPositionEnabled = deserializer.Read(item, "VPosActive", false),
                        IsSizeEnabled = deserializer.Read(item, "SizeActive", false),
                        IsCloneEnabled = deserializer.Read(item, "CloneActive", false),
                        IsCropEnabled = deserializer.Read(item, "CropActive", false),
                        IsBorderEnabled = deserializer.Read(item, "BorderActive", false),
                        IsShadowEnabled = deserializer.Read(item, "ShadowActive", false),
                        IsAspectRatioOffsetEnabled = deserializer.Read(item, "AspectRatioActive", false),
                        IsPanZoomEnabled = deserializer.Read(item, "ZoomPanActive", false),
                        IsTransparencyEnabled = deserializer.Read(item, "TransparencyActive", false)
                    }),

                    //Register Item Lookup
                    (itemList, register) => itemList.FirstOrDefault(t => t.ID == register.LookupID));

        }

        protected List<Router> ParseRouters(XDocument document)
        {
            return ParseList(
                () => document.Descendants("Routers")
                    .Elements("Item")
                    .Select(item => item.Element("Value") ?? item)
                    .Select(item => new Router()
                    {
                        ID = deserializer.Read(item, "ID", -1),
                        Name = deserializer.Read(item, "Name", string.Empty),
                        RouterType = deserializer.Read(item, "RouterType", string.Empty),
                        ConnectorType = deserializer.ReadEnum(item, "ConnectorType", InputConnector.HD15).ToConnectorType(),
                        InputCount = deserializer.Read(item, "Inputs", -1),
                        OutputCount = deserializer.Read(item, "Outputs", -1),
                        Port = deserializer.Read(item, "Port", -1),
                        SerialRouter = deserializer.Read(item, "SerialRouter", false),
                        LevelControlledRouter = deserializer.Read(item, "LevelControlledRouter", false),
                        ControlLevel = deserializer.Read(item, "ControlLevel", 0),
                        LevelCount = deserializer.Read(item, "LevelCount", 0),
                        TransportType = deserializer.Read(item, "TransportType", string.Empty),
                        IPAddress = deserializer.Read(item, "IPAddress", string.Empty),
                        IPRouter = deserializer.Read(item, "IPRouter", false),

                        Patch = new Dictionary<int, RouterPatch>(
                        item.Element("Patch")
                                .Descendants("Item")
                                .ToDictionary(
                                patch => int.Parse(patch.Element("Key").Value),
                                patch =>
                                {
                                    var value = patch.Element("Value");
                                    return new RouterPatch()
                                    {
                                        DownstreamRouterID = deserializer.Read(value, "DownstreamRouterID", -1),
                                        DownstreamRouterInput = deserializer.Read(value, "DownstreamRouterInput", -1),
                                        RouterOutput = deserializer.Read(value, "RouterOutput", -1)
                                    };
                                }))
                    }));
        }

        protected List<InputConfig> ParseInputConfigs(XDocument document)
        {
            return ParseList(
                () => document.Descendants("InputConfigs")
                    .Elements("Item")
                    .Select(item => item.Element("Value") ?? item)
                    .Select(item => new InputConfig()
                    {
                        ID = deserializer.Read(item, "ID", 0),
                        Name = deserializer.Read(item, "Name", string.Empty),
                        AspectRatio = deserializer.Read(item, "AspectRatio", 0f),
                        AutoSyncMode = deserializer.ReadEnum(item, "AutoSync", AutoSyncMode.Manual),
                        Brightness = deserializer.Read(item, "BrightnessMaster", 0),
                        Contrast = deserializer.Read(item, "ContrastMaster", 0),
                        Hue = deserializer.Read(item, "Hue", 0),
                        Saturation = deserializer.Read(item, "Sat", 0),
                        Gamma = deserializer.Read(item, "Gamma", 0),
                        HActive = deserializer.Read(item, "HActive", 0),
                        VActive = deserializer.Read(item, "VActive", 0),
                        HTotal = deserializer.Read(item, "HTotal", 0),
                        VTotal = deserializer.Read(item, "VTotal", 0),
                        UseAlternateInputSynchronizationMethod = deserializer.Read(item, "UseAlternateInputSyncronizationMethod", false),
                        ClockPhase = deserializer.Read(item, "ClockPhase", 0),
                        ColorSpace = deserializer.ReadEnum(item, "ColorSpace", ColorSpace.RGB),
                        ConnectorType = deserializer.ReadEnum(item, "Connector", InputConnector.HD15),
                        CropOffsetBottom = deserializer.Read(item, "CropOffsetBottom", 0),
                        CropOffsetLeft = deserializer.Read(item, "CropOffsetLeft", 0),
                        CropOffsetRight = deserializer.Read(item, "CropOffsetRight", 0),
                        CropOffsetTop = deserializer.Read(item, "CropOffsetTop", 0),
                        DCPulsePosition = deserializer.ReadEnum(item, "DCPulsePos", DCPulsePos.OnSync),
                        DCRestoreMode = deserializer.ReadEnum(item, "DCResMode", DCRestoreMode.Automatic),
                        DCRestorePulseDelay = deserializer.Read(item, "DCResPulseDelay", 0),
                        DCRestorePulseWidth = deserializer.Read(item, "DCResPulseWidth", 0),
                        DetailEnhance = deserializer.Read(item, "DetailEnhance", 0),
                        GenerateStereoSyncWhenMissing = deserializer.Read(item, "GenerateStereoSyncWhenMissing", false),
                        HBackPorch = deserializer.Read(item, "HBackPorch", 0),
                        HDelay = deserializer.Read(item, "HDelay", 0),
                        HHoldOff = deserializer.Read(item, "HHoldOff", 0),
                        HStart = deserializer.Read(item, "HStart", 0),
                        IsEdgeDetect = deserializer.Read(item, "IsEdgeDetect", false),
                        IsInterlaced = deserializer.Read(item, "Interlaced", false),
                        IsVMotionDetect = deserializer.Read(item, "IsVMotionDetect", false),
                        KeyBlueWindow = (byte)deserializer.Read(item, "KeyBlueWindow", 0),
                        KeyClip = deserializer.Read(item, "KeyClip", 0),
                        KeyColor = deserializer.Read(item, "KeyColor", new Color(0, 0, 0)),
                        KeyColorGain = deserializer.Read(item, "KeyColorGain", 0),
                        KeyerMode = deserializer.ReadEnum(item, "KeyerMode", KeyerMode.Off),
                        KeyGain = deserializer.Read(item, "KeyGain", 0),
                        KeyGreenWindow = (byte)deserializer.Read(item, "KeyGreenWindow", 0),
                        KeyRedWindow = (byte)deserializer.Read(item, "KeyRedWindow", 0),
                        NoiseRed = deserializer.Read(item, "NoiseRed", 0),
                        PLLClockPhase = deserializer.Read(item, "PllClockPhase", 0),
                        SOGPickoff = deserializer.Read(item, "SOGPickOff", 0),
                        StereoElevation = deserializer.Read(item, "StereoElevation", 0),
                        StereoInvertEyes = deserializer.Read(item, "StereoInvertEyes", false),
                        StereoMode = deserializer.ReadEnum(item, "StereoMode", InputStereoMode.Off),
                        SyncType = deserializer.ReadEnum(item, "SyncType", InputSyncType.AutoDetect),
                        UseManualPLLClockPhase = deserializer.Read(item, "UseManualPllClockPhase", false),
                        VDelay = deserializer.Read(item, "VDelay", 0),
                        VerticalFrequency = deserializer.Read(item, "VerticalFreq", 0),
                        VHoldOff = deserializer.Read(item, "VHoldOff", 0),
                        VStart = deserializer.Read(item, "VStart", 0)
                    }));
        }

        protected RouterSalvo ParseRouterSalvo(XElement routerSalvoRoot)
        {
            return new RouterSalvo()
            {
                ID = deserializer.Read(routerSalvoRoot, "ID", -1),
                Name = deserializer.Read(routerSalvoRoot, "Name", string.Empty),
                RouterID = deserializer.Read(routerSalvoRoot, "RouterID", -1),

                Salvos = routerSalvoRoot.Element("Salvos")
                    .Descendants("Item")
                    .ToDictionary(
                    salvo => deserializer.Read(salvo, "Key", -1),
                    salvo => deserializer.Read(salvo, "Value", -1))
            };
        }

        protected KeyFrame ParseKeyFrame(XElement keyFrameRoot)
        {
            return new KeyFrame()
            {
                Tension = deserializer.Read(keyFrameRoot, "Tension", 0),
                Bias = deserializer.Read(keyFrameRoot, "Bias", 0),
                Continuity = deserializer.Read(keyFrameRoot, "Cont", 0),
                EaseIn = deserializer.Read(keyFrameRoot, "EaseIn", 0),
                EaseOut = deserializer.Read(keyFrameRoot, "EaseOut", 0),
                Duration = deserializer.Read(keyFrameRoot, "Duration", 0),
                HPosition = deserializer.Read(keyFrameRoot, "HPos", 0),
                VPosition = deserializer.Read(keyFrameRoot, "VPos", 0),
                Width = deserializer.Read(keyFrameRoot, "HSize", 0),
                AspectRatioOffset = deserializer.Read(keyFrameRoot, "AspectRatioOffset", 0),
                Transparency = (byte)deserializer.Read(keyFrameRoot, "Transparency", 0),

                BorderColor = new Color(
                    (byte)deserializer.Read(keyFrameRoot, "BorderRed", 0),
                    (byte)deserializer.Read(keyFrameRoot, "BorderGreen", 0),
                    (byte)deserializer.Read(keyFrameRoot, "BorderBlue", 0)),

                ShadowColor = new Color(
                    (byte)deserializer.Read(keyFrameRoot, "ShadowRed", 0),
                    (byte)deserializer.Read(keyFrameRoot, "ShadowGreen", 0),
                    (byte)deserializer.Read(keyFrameRoot, "ShadowBlue", 0)),

                BorderInsideSoftness = deserializer.Read(keyFrameRoot, "BorderInsideSoftness", 0),
                BorderOutsideSoftness = deserializer.Read(keyFrameRoot, "BorderOutsideSoftness", 0),
                BorderLumaOffsetTop = deserializer.Read(keyFrameRoot, "BorderLumaOSTop", 0),
                BorderLumaOffsetLeft = deserializer.Read(keyFrameRoot, "BorderLumaOSLeft", 0),
                BorderLumaOffsetBottom = deserializer.Read(keyFrameRoot, "BorderLumaOSBot", 0),
                BorderLumaOffsetRight = deserializer.Read(keyFrameRoot, "BorderLumaOSRight", 0),
                BorderThickness = deserializer.Read(keyFrameRoot, "BorderThickness", 0),
                BorderOutsideSoftBottom = deserializer.Read(keyFrameRoot, "BorderOutsideSoftBottom", false),
                BorderOutsideSoftLeft = deserializer.Read(keyFrameRoot, "BorderOutsideSoftLeft", false),
                BorderOutsideSoftRight = deserializer.Read(keyFrameRoot, "BorderOutsideSoftRight", false),
                BorderOutsideSoftTop = deserializer.Read(keyFrameRoot, "BorderOutsideSoftTop", false),

                ShadowHOffset = deserializer.Read(keyFrameRoot, "ShadowHOffset", 0),
                ShadowVOffset = deserializer.Read(keyFrameRoot, "ShadowVOffset", 0),
                ShadowTransparency = deserializer.Read(keyFrameRoot, "ShadowTransparency", 0),
                ShadowSoftness = deserializer.Read(keyFrameRoot, "ShadowSoftness", 0),
                ShadowHSize = deserializer.Read(keyFrameRoot, "ShadowHSize", 0),
                ShadowVSize = deserializer.Read(keyFrameRoot, "ShadowVSize", 0),
                CloneMode = deserializer.ReadEnum(keyFrameRoot, "CloneMode", CloneMode.Off),
                CloneOffset = deserializer.Read(keyFrameRoot, "CloneOffset", 0f),
                CropAnchor = deserializer.ReadEnum(keyFrameRoot, "CropAnchor", CropAnchorTypes.WindowCenter),

                TopCrop = deserializer.Read(keyFrameRoot, "TopCrop", 0f),
                BottomCrop = deserializer.Read(keyFrameRoot, "BottomCrop", 0f),
                LeftCrop = deserializer.Read(keyFrameRoot, "LeftCrop", 0f),
                RightCrop = deserializer.Read(keyFrameRoot, "RightCrop", 0f),

                PanH = deserializer.Read(keyFrameRoot, "PanH", 0),
                PanV = deserializer.Read(keyFrameRoot, "PanV", 0),
                Zoom = deserializer.Read(keyFrameRoot, "Zoom", 0f),
                UseDefaultMotionValues = deserializer.Read(keyFrameRoot, "UseDefaultMotionValues", true),

                BorderFillSource = deserializer.ReadEnum(keyFrameRoot, "BorderFillSource", TextureFillSource.SolidColor),
                BorderTileMode = deserializer.ReadEnum(keyFrameRoot, "BorderTileMode", TextureTileMode.Stretch),
                BorderTextureType = deserializer.ReadEnum(keyFrameRoot, "BorderTextureType", TextureType.Brick_01),
                BorderTextureFile = deserializer.Read(keyFrameRoot, "BorderTextureFile", string.Empty),
                BorderShapeStretch = deserializer.ReadEnum(keyFrameRoot, "BorderShapeStretch", BorderStretchMode.Fill),
                BorderShapeStretchAspectRatio = deserializer.Read(keyFrameRoot, "BorderShapeStretchAR", 1f),

                BorderShapeSource = deserializer.ReadEnum(keyFrameRoot, "BorderShapeSource", ShapeSource.Rectangle),
                BorderShape = deserializer.ReadEnum(keyFrameRoot, "BorderShape", ShapeType.Callout_Fine),
                BorderShapeFile = deserializer.Read(keyFrameRoot, "BorderShapeFile", string.Empty)
            };
        }

        protected List<Register> ParseRegisterList(XDocument document, RegisterType registerType)
        {
            string regType = string.Empty;

            switch (registerType)
            {
                case RegisterType.PlayItem:
                    regType = "PlayItemRegs";
                    break;
                case RegisterType.CommandKey:
                    regType = "CommandKeyRegs";
                    break;
                case RegisterType.Treatment:
                    regType = "TreatmentRegs";
                    break;
                case RegisterType.Source:
                    regType = "SourceRegs";
                    break;
                case RegisterType.FunctionKey:
                    regType = "FunctionKeyRegs";
                    break;
                case RegisterType.Still:
                    regType = "StillRegs";
                    break;
            }

            if (string.IsNullOrEmpty(regType))
                throw new NotSupportedException("Supplied register type is not supported");

            return ParseList(() =>
                {
                    return document.Descendants("RegDataMap")
                        .Descendants(regType)
                        .SelectMany((item) => item.Descendants("Item"))
                        .Select((keyValuePair) =>
                            {
                                int registerID = deserializer.Read(keyValuePair, "Key", -1);
                                XElement item = deserializer.ReadElement(keyValuePair, "Value");
                                if (item == null)
                                    item = keyValuePair;

                                if (registerID == -1)
                                    registerID = deserializer.Read(item, "ID", -1);

                                return new Register()
                                {
                                    RegisterID = registerID,    //deserializer.Read(item, "RegisterID", -1),
                                    Name = deserializer.Read(item, "Name", string.Empty),
                                    Type = registerType,
                                    RegisterColor = deserializer.Read(item, "RegisterColor", new Color(0, 0, 0)),
                                    RegisterColorDefined = deserializer.Read(item, "RegisterColorDefined", false),
                                    LookupID = deserializer.Read(item, "LookupID", -1)
                                };
                            })
                            .Where(r => r.Type == registerType)
                            .ToList();
                });
        }

        protected List<RegisterPage> ParseRegisterPageList(XDocument document, RegisterType registerType)
        {
            string pageTagName = string.Empty;

            switch (registerType)
            {
                case RegisterType.CommandKey:
                    pageTagName = "CommandKeyPages";
                    break;
                case RegisterType.FunctionKey:
                    pageTagName = "FunctionKeyPages";
                    break;
            }

            if (string.IsNullOrEmpty(pageTagName))
                throw new NotSupportedException("Supplied register type is not supported");

            return ParseList(() =>
            {
                return document.Descendants(pageTagName)
                    .Descendants("Item")
                    .Select((item) =>
                    {
                        return new RegisterPage()
                        {
                            PageIndex = deserializer.Read(item, "Key", -1),
                            Name = deserializer.Read(item, "Value", string.Empty)
                        };
                    });
            });
        }

        /// <summary>
        /// Parses items from a list, and appends register properties from the associated register list
        /// </summary>
        protected List<T> ParseList<T>(XDocument document, RegisterType type, Func<IEnumerable<T>> processor, Func<List<T>, IRegister, T> itemLookup) where T : IRegister
        {
            List<T> responseItems = ParseList(processor);
            List<Register> registers = ParseRegisterList(document, type);
            List<T> response = new List<T>();
            
            foreach (Register register in registers)
            {
                T responseItem = itemLookup(responseItems, register);
                if (responseItem != null)
                {
                    //HACK:  Spyder Studio file format is different for command keys, which store their own register color
                    if (responseItem.RegisterColorDefined)
                    {
                        register.RegisterColorDefined = responseItem.RegisterColorDefined;
                        register.RegisterColor = responseItem.RegisterColor;
                    }

                    responseItem.CopyFrom(register);
                    response.Add(responseItem);
                    responseItems.Remove(responseItem);
                }
            }
            return response;
        }

        protected List<T> ParseList<T>(Func<IEnumerable<T>> processor)
        {
            try
            {
                return processor().ToList();
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while parsing {1} list: {2}", ex.GetType().Name, typeof(T).Name, ex.Message);
                return null;
            }
        }

        #region Script File Deserialization

        protected List<Script> ParseScripts(XDocument document)
        {
            return ParseList(
                () => document.Descendants("Scripts")
                    .Elements("Item")
                    .Select(item => item.Element("Value") ?? item)
                    .Select(item => new Script()
                    {
                        ID = deserializer.Read(item, "ID", -1),
                        IsRelative = !deserializer.Read(item, "Absolute", true),
                        Revision = deserializer.Read(item, "Revision", 0),
                        ClockSourceID = deserializer.Read(item, "ClockSourceID", -1),
                        Elements = new List<ScriptElement>(item.Element("Elements").Elements("Item").Select(node => ParseScriptElement(node))),
                        Cues = new List<ScriptCue>(item.Element("CueDatas").Elements().Select(node => ParseScriptCue(node)))
                    }));
        }

        string typeAttributeName;
        private string GetAttributeFromTypeAttribute(XElement node)
        {
            //Depending on the vintange of the call, there may be a value sub-element where the attribute lives
            var valueElement = node?.Element("Value");
            if (valueElement != null)
            {
                string response = GetAttributeFromTypeAttribute(valueElement);
                if (response != null)
                    return response;
            }

            var supportedKeys = new string[] { "typename", "Type" };
            if (typeAttributeName == null)
            {
                foreach (string key in supportedKeys)
                {
                    string response = node.Attributes(key)?.FirstOrDefault()?.Value;
                    if (response != null)
                    {
                        typeAttributeName = key;
                        return response;
                    }
                }
            }
            else
            {
                string response = node.Attributes(typeAttributeName)?.FirstOrDefault()?.Value;
                return response;
            }

            return null;
        }

        protected ScriptElement ParseScriptElement(XElement node)
        {
            string elementType = GetAttributeFromTypeAttribute(node);
            if (elementType == null)
                throw new NotSupportedException("Invalid XML markup for script element - unable to parse");

            node = node.Element("Value");

            if (elementType.Contains("ElementSource"))
            {
                return ParseScriptElement<SourceElement>(node, null);
            }
            else if (elementType.Contains("ElementMixer"))
            {
                return ParseScriptElement(node, (MixerElement e) =>
                    {
                        e.ThruBlack = deserializer.Read(node, "ThruBlack", false);
                        e.Morph = deserializer.Read(node, "Morph", true);
                    });
            }
            else if (elementType.Contains("InputArray"))
            {
                return ParseScriptElement<InputArrayElement>(node, null);
            }
            else if (elementType.Contains("Still"))
            {
                return ParseScriptElement(node, (StillElement e) =>
                    {
                        //Versions below 5.2 will have a statically assigned filename instead of content on a still element
                        if(node.Element("FileName") != null && e.Contents.Count == 0)
                        {
                            //Create a dummy content item for this still
                            e.Contents.Add(0, new Content()
                            {
                                Type = ContentType.Still,
                                Name = deserializer.Read(node, "FileName", string.Empty)
                            });
                        }
                    });
            }
            else if (elementType.Contains("ElementOff"))
            {
                return ParseScriptElement<OffElement>(node, null);
            }
            else
            {
                throw new NotSupportedException(string.Format("Element type {0} is unrecognized", elementType));
            }
        }

        protected T ParseScriptElement<T>(XElement node, Action<T> parser) where T : ScriptElement, new()
        {
            //Deserialize base properties
            var response = new T()
            {
                Name = deserializer.Read(node, "Name", string.Empty),
                Thumbnail = deserializer.Read(node, "ThumbNail", string.Empty),
                PixelSpaceID = deserializer.Read(node, "PixelSpaceID", 0),
                StartLayer = deserializer.Read(node, "StartLayer", 0),
                LayerCount = deserializer.Read(node, "NumLayers", 0),
                StartCue = deserializer.Read(node, "StartCue", 0),
                CueCount = deserializer.Read(node, "NumCues", 0),
                MixOnRate = deserializer.Read(node, "MixOnRate", 0),
                MixOffRate = deserializer.Read(node, "MixOffRate", 0),
                EntranceEffectID = deserializer.Read(node, "OnEffectID", 0),
                ExitEffectID = deserializer.Read(node, "OffEffectID", 0),
                EntranceEffectType = deserializer.ReadEnum(node, "OnEffectType", SlideTransitionType.None),
                ExitEffectType = deserializer.ReadEnum(node, "OffEffectType", SlideTransitionType.None),
                IsDisabled = deserializer.Read(node, "Disabled", false),
                KeyFrames = node.Element("KeyFrames").Descendants("Item").ToDictionary(
                    (item => deserializer.Read(item, "Key", -1)),
                    (item => ParseKeyFrame(item.Element("Value") ?? item)))
            };

            //Spyder studio (5.2+) refers to content more generically as content, where older versions maintain a string source list
            if (node.Element("Sources") != null)
            {
                //V4 style
                response.Contents = node.Element("Sources").Descendants("Item").ToDictionary(
                    (item => deserializer.Read(item, "Key", -1)),
                    (item => new Content() { Type = ContentType.Source, Name = deserializer.Read(item, "Value", string.Empty) }));
            }
            else if (node.Element("Contents") != null)
            {
                //V5 style
                response.Contents = node.Element("Contents").Descendants("Item").ToDictionary(
                    (item => deserializer.Read(item, "Key", -1)),
                    (item) =>
                    {
                        var val = item.Element("Value");
                        return new Content()
                        {
                            Type = (ContentType)Enum.Parse(typeof(ContentType), deserializer.Read(val, "Type", "Source")),
                            Name = deserializer.Read(val, "Name", string.Empty)
                        };
                    });
            }
            else
            {
                throw new InvalidDataException("Unable to find element content");
            }

            //Let the passed in handler parse the rest of this object
            if (parser != null)
                parser(response);

            return response;
        }

        protected ScriptCue ParseScriptCue(XElement node)
        {
            var cue = node.Element("Value") ?? node;

            return new ScriptCue()
            {
                ID = deserializer.Read(cue, "ID", -1),
                Name = deserializer.Read(cue, "Name", string.Empty),
                JumpTo = deserializer.Read(cue, "JumpTo", -1),
                JumpType = deserializer.ReadEnum(cue, "JumpType", JumpType.None),
                TriggerTime = deserializer.Read(cue, "TriggerTime", new TimeCode()),
                TriggerType = deserializer.ReadEnum(cue, "TriggerType", CueTrigger.None),

                PlayItems = new List<KeyValuePair<int, PlayItemCommand>>(
                    cue.Element("PlayItems").Descendants("Item").Select(pi =>
                        new KeyValuePair<int, PlayItemCommand>(
                            deserializer.Read(pi, "Key", -1),
                            deserializer.ReadEnum(pi, "Value", PlayItemCommand.Cue)))),

                FunctionKeyIDs = new List<int>(cue.Element("FunctionKeyIDs").Descendants("Item").Select(
                    item => int.Parse(item.Value))),
            };
        }

        #endregion
    }
}
