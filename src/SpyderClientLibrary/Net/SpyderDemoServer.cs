using Knightware.Primitives;
using Knightware.Threading.Tasks;
using Spyder.Client.Common;
using Spyder.Client.FunctionKeys;
using Spyder.Client.Net.DrawingData;
using Spyder.Client.Net.Notifications;
using Spyder.Client.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    /// <summary>
    /// Container class for a mock dataset for a Spyder server
    /// </summary>
    internal class SpyderDemoServer
    {
        private readonly SystemData data;
        private readonly DrawingData.DrawingData drawingData;
        private readonly AutoResetWorker drawingDataUpdateWorker;

        private int lastCommandKeyRegisterIDRecalled = -1;
        private int lastCommandKeyCueRecalled = -1;
        private int lastCommandKeyRegisterIDProcessed = -1;
        private int lastCommandKeyCueProcessed = -1;

        private TimeSpan drawingDataThrottleInterval = TimeSpan.Zero;
        public TimeSpan DrawingDataThrottleInterval
        {
            get { return drawingDataThrottleInterval; }
            set
            {
                drawingDataThrottleInterval = value;
                drawingDataUpdateWorker.PeriodicSignallingTime = (value == TimeSpan.Zero ? TimeSpan.FromSeconds(1) : value);
            }
        }

        public event DrawingDataReceivedHandler DrawingDataReceived;
        protected void OnDrawingDataReceived(DrawingDataReceivedEventArgs e)
        {
            if (DrawingDataReceived != null)
                DrawingDataReceived(this, e);
        }

        public SpyderDemoServer(SystemData data)
        {
            this.data = data;
            this.drawingData = CreateDrawingDataFromSystemData(data);

            //Load an intial scene if available
            if (data.CommandKeys.Count > 0)
            {
                lastCommandKeyRegisterIDRecalled = data.CommandKeys[0].RegisterID;
                lastCommandKeyCueRecalled = 1;
            }

            //Every second, refresh and re-announce drawing data
            drawingDataUpdateWorker = new AutoResetWorker();
            drawingDataUpdateWorker.PeriodicSignallingTime = TimeSpan.FromSeconds(1);
            Task t = drawingDataUpdateWorker.StartupAsync(drawingDataUpdateWorker_DoWork, (Func<bool>)(() => (DrawingDataReceived != null)));
        }

        private Task drawingDataUpdateWorker_DoWork(object state)
        {
            if (lastCommandKeyRegisterIDRecalled != -1)
            {
                if (lastCommandKeyRegisterIDRecalled != lastCommandKeyRegisterIDProcessed || lastCommandKeyCueRecalled != lastCommandKeyCueProcessed)
                {
                    lastCommandKeyRegisterIDProcessed = lastCommandKeyRegisterIDRecalled;
                    lastCommandKeyCueProcessed = lastCommandKeyCueRecalled;
                    UpdateDrawingDataFromCommandKey(lastCommandKeyRegisterIDProcessed, lastCommandKeyCueProcessed);
                }
            }

            //Raise change notification
            OnDrawingDataReceived(new DrawingDataReceivedEventArgs("Demo", drawingData));

            return Task.FromResult(true);
        }

        public VersionInfo GetVersionInfo()
        {
            return new VersionInfo(4, 0, 1);
        }

        #region Register / Data List Access

        public List<RegisterPage> GetRegisterPages(RegisterType type)
        {
            if (type == RegisterType.FunctionKey)
                return data.FunctionKeyPages;
            else if (type == RegisterType.CommandKey)
                return data.CommandKeyPages;
            else
                return null;
        }

        public IEnumerable<IRegister> GetRegisters(RegisterType type)
        {
            switch (type)
            {
                case RegisterType.PlayItem:
                    return data.PlayItems;

                case RegisterType.CommandKey:
                    return data.CommandKeys;

                case RegisterType.Treatment:
                    return data.Treatments;

                case RegisterType.Source:
                    return data.Sources;

                case RegisterType.FunctionKey:
                    return data.FunctionKeys;

                case RegisterType.Still:
                    return data.Stills;

                default:
                    return new List<IRegister>();
            }
        }

        public IRegister GetRegister(RegisterType type, int registerID)
        {
            return GetRegisters(type).FirstOrDefault(register => register.RegisterID == registerID);
        }

        public RegisterType? GetRegisterType<T>()
        {
            Type t = typeof(T);
            if (t.Equals(typeof(Source)))
            {
                return RegisterType.Source;
            }
            else if (t.Equals(typeof(Treatment)))
            {
                return RegisterType.Treatment;
            }
            else if (t.Equals(typeof(CommandKey)))
            {
                return RegisterType.CommandKey;
            }
            else if (t.Equals(typeof(FunctionKey)))
            {
                return RegisterType.FunctionKey;
            }
            else if (t.Equals(typeof(PlayItem)))
            {
                return RegisterType.PlayItem;
            }
            else if (t.Equals(typeof(Still)))
            {
                return RegisterType.Still;
            }
            else
            {
                return null;
            }
        }

        public List<T> GetList<T>(RegisterType type) where T : IRegister
        {
            RegisterType? regType = GetRegisterType<T>();
            if (regType == null)
                return null;

            return GetRegisters(regType.Value).Cast<T>().ToList();
        }

        public T GetListItem<T>(IRegister register) where T : IRegister
        {
            return GetListItem<T>(register.Type, register.RegisterID);
        }

        public T GetListItem<T>(RegisterType type, int registerID) where T : IRegister
        {
            var itemlist = GetList<T>(type);
            if (itemlist == null)
                return default(T);

            return itemlist.FirstOrDefault(item => item.RegisterID == registerID);
        }

        public List<Source> GetSources()
        {
            return data.Sources;
        }

        public Source GetSource(string name)
        {
            return data.Sources.FirstOrDefault(s => s.Name == name);
        }

        public bool DeleteCommandKey(params int[] commandKeyRegisterIDs)
        {
            if (data.CommandKeys != null)
            {
                var keys = data.CommandKeys.Where(c => commandKeyRegisterIDs.Contains(c.RegisterID)).ToList();
                foreach (var key in keys)
                    data.CommandKeys.Remove(key);
            }
            return true;
        }

        public List<Script> GetScripts()
        {
            return data.Scripts;
        }

        public Script GetScript(int scriptID)
        {
            return data.Scripts.FirstOrDefault(s => s.ID == scriptID);
        }

        public List<PixelSpace> GetPixelSpaces()
        {
            return data.PixelSpaces;
        }

        public PixelSpace GetPixelSpace(int pixelSpaceID)
        {
            return data.PixelSpaces.FirstOrDefault(ps => ps.ID == pixelSpaceID);
        }

        public List<InputConfig> GetInputConfigs()
        {
            return data.InputConfigs;
        }

        public InputConfig GetInputConfig(int InputConfigID)
        {
            return data.InputConfigs.FirstOrDefault(ic => ic.ID == InputConfigID);
        }

        public InputConfig GetInputConfig(string sourceName)
        {
            Source source = GetSource(sourceName);
            if (source != null)
            {
                return GetInputConfig(source.InputConfigID);
            }
            return null;
        }

        public List<Shape> GetShapes()
        {
            int index = 0;
            var response = new List<Shape>();
            foreach (var shape in Shape.GetFactoryShapes())
            {
                response.Add(new Shape()
                {
                    Name = string.Format("Shape File {0}.shape", index++),
                    Definition = shape.Definition
                });
            }
            return response;
        }

        public Shape GetShape(string shapeFileName)
        {
            return new Shape()
            {
                Name = shapeFileName,
                Definition = Shape.GetFactoryShape(ShapeType.Callout_Fine).Definition
            };
        }

        public bool SetShape(Shape shape)
        {
            return true;
        }

        public List<string> GetShapeFileNames()
        {
            int index = 0;
            var response = new List<string>();
            foreach (var shape in Shape.GetFactoryShapes())
            {
                response.Add(string.Format("Shape File {0}.shape", index++));
            }
            return response;
        }

        public ServerSettings GetServerSettings()
        {
            return new ServerSettings()
            {
                FieldRate = FieldRate.NTSC,
                FrameID = 0,
                UserDiagnosticMonitoringEnabled = true
            };
        }

        #endregion

        private bool DoOperationToLayer(Action<DrawingKeyFrame> action, params int[] layerIDs)
        {
            if (layerIDs != null && layerIDs.Length > 0)
            {
                foreach (int layerID in layerIDs)
                {
                    var layer = drawingData.GetLayer(layerID, MixerBus.Program);
                    if (layer != null)
                        action(layer);
                }
            }
            return true;
        }

        #region Layer Interaction

        public bool ApplyRegisterToLayer(RegisterType type, int registerID, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        public bool FreezeLayer(params int[] layerIDs)
        {
            return DoOperationToLayer((l) => l.IsFrozen = true, layerIDs);
        }

        public bool UnFreezeLayer(params int[] layerIDs)
        {
            return DoOperationToLayer((l) => l.IsFrozen = false, layerIDs);
        }

        public bool MixOffAllLayers(int duration)
        {
            return DoOperationToLayer((l) => l.Transparency = 255,
                drawingData.DrawingKeyFrames
                .Where(l => !l.Value.IsBackground)
                .Select(l => l.Key)
                .ToArray());
        }

        public bool MixOffLayer(int duration, params int[] layerIDs)
        {
            return DoOperationToLayer((l) => l.Transparency = 255, layerIDs);
        }

        public bool MixOnLayer(int duration, params int[] layerIDs)
        {
            return DoOperationToLayer((l) => l.Transparency = 0, layerIDs);
        }

        public int GetFirstAvailableLayer()
        {
            foreach (var layer in drawingData.DrawingKeyFrames.Values)
            {
                if (!layer.IsVisible && !layer.IsBackground)
                    return layer.LayerID;
            }
            return -1;
        }

        public int RequestLayerCount()
        {
            return drawingData.DrawingKeyFrames.Where(l => !l.Value.IsBackground).Count();
        }

        public bool MixOnLayer(int pixelSpaceID, Point position, int width, int duration, Register content)
        {
            return MixOnLayer(GetFirstAvailableLayer(), pixelSpaceID, position, width, duration, content);
        }

        public bool MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, int duration, Register content)
        {
            //TODO:  Implement me
            return true;
        }

        public bool TransitionLayer(int duration, params int[] layerIDs)
        {
            //TODO: transition
            return true;
        }

        public bool ResizeLayer(LayerResizeType type, int newWidth, params int[] layerIDs)
        {
            //TODO:  Wire me in
            return true;

            //return DoOperationToLayer((layer) =>
            //    {
            //        int widthDelta = (type == LayerResizeType.AbsolutePixel ?
            //            newWidth - layer.LayerRect.Width :
            //            layer.LayerRect.Width + newWidth);
            //        int halfWidthDelta = halfWidthDelta = widthDelta / 2;

            //        layer.LayerRect = new Primitives.Rectangle()
            //        {
            //            X = layer.LayerRect.X - halfWidthDelta,
            //            Y = layer.LayerRect.Y,
            //            Width = layer.LayerRect.Width + widthDelta,
            //            Height = layer.LayerRect.Height
            //        };

            //        layer.CloneRect = new Primitives.Rectangle()
            //        {
            //            X = layer.CloneRect.X - halfWidthDelta,
            //            Y = layer.CloneRect.Y,
            //            Width = layer.CloneRect.Width + widthDelta,
            //            Height = layer.CloneRect.Height
            //        };
            //    }, layerIDs);
        }

        public bool ResizeLayer(LayerResizeType resizeType, LayerResizeDirection resizeDirection, double value, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        public bool MoveLayer(LayerMoveType moveType, double hPosition, double vPosition, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        public bool MoveAndResizeLayer(MoveAndResizeType moveType, int hPosition, int vPosition, int hSize, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        #region Treatments

        public bool LearnTreatment(int treatmentID, int layerID, bool learnPosition, bool learnCrop, bool learnClone, bool learnBorder, bool learnShadow)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Sets outside softness on the layer.  When a non-zero value is entered, border settings will be disabled
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="outsideSoftnessThickness">Outside softness in pixels (0-255)</param>
        /// <returns></returns>
        public bool AdjustLayerOutsideSoftness(int layerID, int outsideSoftnessThickness)
        {
            //TODO:  Implement me
            return true;
        }

        public bool AdjustLayerBorderBevel(int hBevel, int vBevel, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        public bool AdjustLayerBorderColor(Color borderColor, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        public bool AdjustLayerBorder(int layerID, int borderThickness)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        /// <param name="borderColor">Border color</param>
        public bool AdjustLayerBorder(int layerID, int borderThickness, Color borderColor)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        /// <param name="borderColor">Border color</param>
        /// <param name="hBevel">Luminance offset for left/right border edges (0-255)</param>
        /// <param name="vBevel">Luminance offset for top/bottom border edges (0-255)</param>
        public bool AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        /// <param name="borderColor">Border color</param>
        /// <param name="hBevel">Luminance offset for left/right border edges (0-255)</param>
        /// <param name="vBevel">Luminance offset for top/bottom border edges (0-255)</param>
        /// <param name="insideSoftness">Number of pixels to blend the border into the source image (0-255)</param>
        /// <returns></returns>
        public bool AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel, int insideSoftness)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        public bool AdjustLayerShadow(int layerID, int hPosition, int vPosition)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        /// <param name="size">Width of shadow, added to the width of the layer (0-255)</param>
        public bool AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        /// <param name="size">Width of shadow, added to the width of the layer (0-255)</param>
        /// <param name="transparency">Visibility of shadow (0-255)</param>
        public bool AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        /// <param name="size">Width of shadow, added to the width of the layer (0-255)</param>
        /// <param name="transparency">Visibility of shadow (0-255)</param>
        /// <param name="outsideSoftness">Number of pixels for the outside softness blend (0-255)</param>
        public bool AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency, int outsideSoftness)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Applies crop parameters to one or more specified layers
        /// </summary>
        /// <param name="leftCrop">Percentage of left crop to set on the layer (0.0 to 1.0)</param>
        /// <param name="rightCrop">Percentage of right crop to set on the layer (0.0 to 1.0)</param>
        /// <param name="topCrop">Percentage of top crop to set on the layer (0.0 to 1.0)</param>
        /// <param name="bottomCrop">Percentage of bottom crop to set on the layer (0.0 to 1.0)</param>
        /// <param name="layerIDs">Layer ID(s) to apply crop settings to</param>
        /// <returns></returns>
        public bool AdjustLayerCrop(double? leftCrop, double? rightCrop, double? topCrop, double? bottomCrop, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        public bool AdjustZoomPan(int layerID, AdjustmentType type, double zoom, int horizontalPan, int verticalPan)
        {
            //TODO: Implement me
            return true;
        }

        public bool AdjustLayerAspectRatio(AspectRatioAdjustmentType type, double aspectRatioValue, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        /// <summary>
        /// Assigns one or more layers to a pixelspace
        /// </summary>
        /// <param name="pixelSpaceID">PixelSpace ID to assign layer(s) to</param>
        /// <param name="makeLayerVisible">When true, layer(s) will </param>
        /// <param name="layerIDs"></param>
        /// <returns></returns>
        public bool LayerAssignPixelSpace(int pixelSpaceID, bool makeLayerVisible, params int[] layerIDs)
        {
            //TODO:  Implement me
            return true;
        }

        #endregion

        #endregion

        #region PixelSpace Interaction

        public bool MixBackground(int duration)
        {
            if (drawingData != null)
            {
                foreach (var pixelSpace in drawingData.PixelSpaces.Values)
                {
                    var temp = pixelSpace.LastBackgroundStill;
                    pixelSpace.LastBackgroundStill = pixelSpace.NextBackgroundStill;
                    pixelSpace.NextBackgroundStill = temp;
                }
            }
            return true;
        }

        public Task<bool> LoadBackgroundImage(int pixelSpaceID, BackgroundImageBus bus, string fileName)
        {
            //Update PixelSpace data
            var pixelSpace = GetPixelSpace(pixelSpaceID);
            if (pixelSpace != null)
            {
                if (bus == BackgroundImageBus.CurrentBackground)
                    pixelSpace.LastBackgroundStill = fileName;
                else
                    pixelSpace.NextBackgroundStill = fileName;
            }

            //Update DrawingData
            if (drawingData != null)
            {
                pixelSpace = drawingData.GetPixelSpace(pixelSpaceID);
                if (pixelSpace != null)
                {
                    if (bus == BackgroundImageBus.CurrentBackground)
                        pixelSpace.LastBackgroundStill = fileName;
                    else
                        pixelSpace.NextBackgroundStill = fileName;
                }
            }

            return Task.FromResult(true);
        }

        public PixelSpace RequestPixelSpace(int pixelSpaceID)
        {
            if (drawingData == null)
                return null;
            else
                return drawingData.GetPixelSpace(pixelSpaceID);
        }

        public List<PixelSpace> RequestPixelSpaces()
        {
            if (drawingData == null)
                return null;
            else
                return new List<PixelSpace>(drawingData.PixelSpaces.Values);
        }

        #endregion

        public bool LearnCommandKey(int pageIndex, int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative)
        {
            //TODO: Implement me
            return true;
        }

        public bool LearnCommandKey(int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative)
        {
            //TODO: Implement me
            return true;
        }

        public bool RecallCommandKey(int registerID, int cueIndex)
        {
            lastCommandKeyRegisterIDRecalled = registerID;
            lastCommandKeyCueRecalled = cueIndex;
            drawingDataUpdateWorker.Set();
            return true;
        }
        public bool RecallFunctionKey(int registerID)
        {
            return true;
        }
        public bool RecallRegisterToLayer(RegisterType registerType, int registerID, params int[] layerIDs)
        {
            return true;
        }

        private DrawingData.DrawingData CreateDrawingDataFromSystemData(SystemData data, int layerCount = 6)
        {
            var response = new DrawingData.DrawingData();
            response.IsDataIOIdle = true;

            //Add PixelSpaces
            foreach (var pixelSpace in data.PixelSpaces)
            {
                response.PixelSpaces.Add(pixelSpace.ID, new DrawingPixelSpace(pixelSpace));
            }

            //Add preview pixelspaces
            foreach (var previewPixelSpace in data.PreviewPixelSpaces)
            {
                response.PreviewPixelSpaceIDs.Add(previewPixelSpace.Key, previewPixelSpace.Value);
            }

            //Add layers
            for (int i = 0; i < layerCount + 2; i++)
            {
                response.DrawingKeyFrames.Add(i, new DrawingKeyFrame()
                {
                    LayerID = i,
                    IsBackground = (i < 2)
                });
            }

            //TODO:  Add hardware preview layers?

            //TODO:  Add Outputs (Needs to be deserialized into system data)?

            response.Frames.Add(0, new DrawingFrame()
            {
                FrameID = 0,
                FrameAOR = new Knightware.Primitives.Rectangle(0, 0, 2560, 1024),
                ProgramAOR = new Knightware.Primitives.Rectangle(0, 0, 2560, 1024),
                Model = SpyderModels.Spyder_365,
            });

            //Add user diagnostic states
            foreach (DiagnosticType type in Enum.GetValues(typeof(DiagnosticType)))
            {
                response.DiagnosticWarnings.Add(type, DiagnosticStatus.Normal);
            }

            return response;
        }

        private void UpdateDrawingDataFromCommandKey(int registerID, int cueIndex)
        {
            var commandKey = data.CommandKeys.FirstOrDefault(c => c.RegisterID == registerID);
            if (commandKey == null)
                return;

            var script = data.Scripts.FirstOrDefault(s => s.ID == commandKey.LookupID);
            if (script == null)
                return;

            //If this is an absolute script, hide all layers now
            if (script.IsRelative)
            {
                //Hide layers associated with this script
                foreach (var element in script.Elements)
                {
                    for (int i = element.StartLayer; i < (element.StartLayer + element.LayerCount); i++)
                    {
                        if (drawingData.DrawingKeyFrames.ContainsKey(i))
                            drawingData.DrawingKeyFrames[i].Transparency = 255;
                    }
                }
            }
            else
            {
                //Hide all layers
                foreach (var dkf in drawingData.DrawingKeyFrames.Values)
                    dkf.Transparency = 255;
            }

            //Setup our script cues
            foreach (var element in script.GetElements(cueIndex))
            {
                var dkf = drawingData.GetLayer(element.StartLayer, MixerBus.Program);
                if (element is OffElement || dkf == null)
                    continue;

                int pixelSpaceID = ((element.StartCue - 1) == cueIndex ? drawingData.GetPreviewPixelSpaceID(element.PixelSpaceID) : element.PixelSpaceID);
                var pixelSpace = GetPixelSpace(pixelSpaceID);
                if (pixelSpace == null)
                    continue;

                InputConfig ic = new InputConfig() { HActive = 1920, VActive = 1080, AspectRatio = 1.7777f };

                //Set type specific properties
                if (element is SourceElement || element is MixerElement)
                {
                    dkf.Source = element.GetDrivingContent(cueIndex, ElementIndexRelativeTo.ParentScript)?.Name;
                    dkf.LoadedStill = string.Empty;
                    dkf.WindowLabel = (element.Contents.Count > 0 ? element.Contents.Values.First().Name : element.Name);

                    var source = GetSource(dkf.Source);
                    if (source != null)
                    {
                        dkf.Thumbnail = source.Thumbnail;
                        if (source.InputConfigID != -1)
                        {
                            InputConfig sourceIC = GetInputConfig(source.InputConfigID);
                            if (sourceIC != null)
                                ic = sourceIC;
                        }
                    }
                }
                else if (element is StillElement stillElement)
                {
                    dkf.Source = string.Empty;
                    dkf.LoadedStill = stillElement.FileName;
                    dkf.WindowLabel = dkf.LoadedStill;
                    dkf.Thumbnail = dkf.LoadedStill;
                }

                //Set global element properties
                dkf.Transparency = 0;
                dkf.LastScript = script.ID;
                dkf.LastCue = cueIndex;
                dkf.KeyFrame.CopyFrom(element.GetDrivingKeyFrame(cueIndex, ElementIndexRelativeTo.ParentScript));
                dkf.PixelSpaceID = pixelSpaceID;
                dkf.LayerRect = LayerHelpers.GetAbsoluteRectangle(dkf.KeyFrame, ic.AspectRatio, pixelSpace);
                dkf.CloneRect = LayerHelpers.GetCloneRectangle(dkf.KeyFrame, pixelSpace.Rect, dkf.LayerRect);
                dkf.AOIRect = new Knightware.Primitives.Rectangle(0, 0, ic.HActive, ic.VActive);
                dkf.HActive = ic.HActive;
                dkf.VActive = ic.VActive;
            }
        }
    }
}
