using Knightware.Diagnostics;
using Knightware.Primitives;
using Spyder.Client.Common;
using Spyder.Client.IO;
using System;
using System.IO;
using System.IO.Pipes;

namespace Spyder.Client.Net.DrawingData.Deserializers
{
    /// <summary>
    /// Deserializes DrawingData messages in the version 64 serialization format - SpyderS 6.0.1
    /// </summary>
    public class DrawingDataDeserializer_Version64 : IDrawingDataDeserializer
    {
        private readonly string serverVersion;
        public DrawingDataDeserializer_Version64(string serverVersion)
        {
            this.serverVersion = serverVersion;
        }

        public virtual DrawingData Deserialize(byte[] stream)
        {
            if (stream == null || stream.Length == 0)
                return null;

            DrawingData response = new DrawingData();
            int index = 0;

            int newPsCount = stream[index++];
            int newDkCount = stream[index++];
            int newPvwDkCount = stream[index++];
            int newMixEffectCount = stream[index++];
            int newRtrCount = stream[index++];
            int newOutputCount = stream[index++];
            int previewPixelSpaceCount = stream[index++];
            int stereoPixelSpaceCount = stream[index++];
            int newMachineCount = stream[index++];
            int frameCount = stream[index++];
            int runningScriptCount = stream[index++];
            int diagnosticWarningCount = stream[index++];
            int configOutputCount = stream[index++];
            int inputCount = stream[index++];

            //Global values
            response.TimeOfDay = new TimeSpan(
                stream[index++],
                stream[index++],
                stream[index++]
                );

            response.PercentComplete = stream[index++];
            response.LastFrontPanelMsg = (FrontPanelDisplayCommand)stream[index++];
            response.ProgressString = stream.GetString(ref index);
            response.ConfigSource = stream.GetString(ref index);
            response.ConfigLayer = stream.GetInt(ref index);
            response.ConfigBus = ParseSpyderSMixerBus(stream[index++]);

            response.HardwareType = (HardwareType)stream[index++];
            index++; //Ignore StereoMode
            response.DataObjectVersion = stream.GetInt(ref index);
            response.DataObjectLastChangeType = (DataType)stream.GetInt(ref index);

            //Global Flags
            byte flags = stream[index++];
            response.OpMonOverlay = (flags & 0x01) > 0;
            response.IsMachineHalEnabled = (flags & 0x02) > 0;
            response.IsRouterHalEnabled = (flags & 0x04) > 0;
            response.IsDataIOIdle = (flags & 0x08) > 0;
            response.IsPreviewOnlyScriptingEnabled = (flags & 0x10) > 0;
            response.IsLiveUpdateEnabled = (flags & 0x20) > 0;

            flags = stream[index++];
            response.LiveUpdatesTemporarilyDisabled = (flags & 0x01) > 0;
            response.IsHdcpEnabled = (flags & 0x02) > 0;

            //System Frame Rate
            response.SystemFrameRate = ParseSpyderSFieldRate(stream[index++]);

            //System Layer Mode
            response.SystemLayerMode = (SystemLayerMode)stream[index++];

            //Get the frames
            for (int i = 0; i < frameCount; i++)
            {
                int frameID = stream[index++];
                var frame = new DrawingFrame()
                {
                    FrameID = frameID,
                    FrameAOR = stream.GetRectangle(ref index),
                    Model = SpyderModelFromByte(stream[index++]),
                    RenewalMasterFrameID = stream[index++],
                };
                //Note: Ignoring model capabilities flag for now
                index++;

                frame.ProgramAOR = frame.FrameAOR; //Spyder-S doesn't require PGM/PVW AORs
                frame.PreviewAOR = frame.FrameAOR;
                response.Frames.Add(frameID, frame);
            }

            //Get the running scripts
            for (int i = 0; i < runningScriptCount; i++)
            {
                int scriptID = stream.GetInt(ref index);
                long countDown = stream.GetLong(ref index);
                response.RunningScripts.Add(scriptID, countDown);
            }

            //Get the preview pixelspace list
            for (int i = 0; i < previewPixelSpaceCount; i++)
            {
                int pgmID = stream.GetShort(ref index);
                int pvwID = stream.GetShort(ref index);
                response.PreviewPixelSpaceIDs.Add(pgmID, pvwID);
            }

            //Get the stereo pixelspace list
            for (int i = 0; i < stereoPixelSpaceCount; i++)
            {
                int leftEyeID = stream.GetShort(ref index);
                int rightEyeID = stream.GetShort(ref index);
                response.StereoPixelSpaceIDs.Add(leftEyeID, rightEyeID);
            }

            //Get all PixelSpaces
            for (int i = 0; i < newPsCount; i++)
            {
                //Create the new pixelspace
                DrawingPixelSpace newPS = new DrawingPixelSpace();

                //Fill the new pixelspace
                newPS.Rect = stream.GetRectangle(ref index);

                //Versions above 5.0.x (5.1.x / 5.2.x / etc) no longer have a RenewalMasterFrameID
                newPS.RenewMasterFrameID = -1;

                newPS.ID = stream.GetShort(ref index);
                MixerBus bus = ParseSpyderSMixerBus(stream[index++]);
                newPS.Name = stream.GetString(ref index);

                //Note:  We'll come back after we get the mix effects below to try and set background content thumbnails

                //PixelSpace flags
                flags = stream[index++];
                newPS.StereoMode = (flags & 0x01) == 0x01 ? PixelSpaceStereoMode.Stereo : PixelSpaceStereoMode.Off;

                //Version 5.0.3+ no longer has PixelSpace scale factor.  For backwards compatibility we'll set either 1f for PGM or .99 for PVW.
                //In the future if this turns out to be problematic we could calculate an effective scale below after we've deserialized all pixelspaces
                newPS.Scale = bus == MixerBus.Program ? 1f : .99f;

                //Add Pixelspace to the list
                response.PixelSpaces.Add(newPS.ID, newPS);
            }

            //Get our mix effects
            for (int i = 0; i < newMixEffectCount; i++)
            {
                DrawingMixEffect mixEffect = new DrawingMixEffect();
                mixEffect.ID = stream.GetInt(ref index);
                mixEffect.Name = stream.GetString(ref index);
                mixEffect.Type = (MixEffectType)stream[index++];
                mixEffect.BottomContentName = stream.GetString(ref index);
                mixEffect.BottomContentThumbnail = stream.GetString(ref index);
                mixEffect.TopContentName = stream.GetString(ref index);
                mixEffect.TopContentThumbnail = stream.GetString(ref index);
                mixEffect.TopContentOpacity = stream.GetFloat(ref index);
                mixEffect.BackgroundPixelSpaceId = stream.GetInt(ref index);

                //Usages
                int usageCount = stream[index++];
                for (int j = 0; j < usageCount; j++)
                {
                    mixEffect.Usages.Add(new DrawingMixEffectUsage()
                    {
                        SourceID = stream.GetInt(ref index),
                        UsageType = (DrawingMixEffectUsageType)stream[index++],
                        Label = stream.GetString(ref index),
                    });
                }

                //Flags
                byte mixEffectFlags = stream[index++];
                mixEffect.TopIsFrozen = (mixEffectFlags & 0x01) > 0;
                mixEffect.BottomIsFrozen = (mixEffectFlags & 0x02) > 0;
                mixEffect.TopSupportsFreeze = (mixEffectFlags & 0x04) > 0;
                mixEffect.BottomSupportsFreeze = (mixEffectFlags & 0x08) > 0;

                response.DrawingMixEffects.Add(mixEffect.ID, mixEffect);
            }

            //Now that we have MixEffects, update pixelspace background info
            foreach (var ps in response.PixelSpaces.Values)
            {
                foreach (var me in response.DrawingMixEffects.Values)
                {
                    if (me.BackgroundPixelSpaceId == ps.ID)
                    {
                        ps.NextBackgroundStillIsOnLayer1 = true;
                        ps.LastBackgroundStill = me.BottomContentThumbnail;
                        ps.NextBackgroundStill = me.TopContentThumbnail;
                        ps.Layer1Transparency = (byte)(me.TopContentOpacity * byte.MaxValue);
                    }
                }
            }

            //Get all of our DrawingKeyFrames
            int allLayerCount = newDkCount + newPvwDkCount;

            //Add a couple 'virtual' background layers to maintain overall compatibility in library.  Spyder-S removed these and starts layer IDs at zero
            response.DrawingKeyFrames.Add(0, new DrawingKeyFrame() { LayerID = 0, IsBackground = true });
            response.DrawingKeyFrames.Add(1, new DrawingKeyFrame() { LayerID = 1, IsBackground = true });

            for (int i = 0; i < allLayerCount; i++)
            {
                DrawingKeyFrame l = new DrawingKeyFrame();
                KeyFrame kf = l.KeyFrame;

                l.FrameID = stream[index++];

                l.MixEffectID = stream.GetInt(ref index);
                if(response.DrawingMixEffects.ContainsKey(l.MixEffectID))
                {
                    l.MixEffect = response.DrawingMixEffects[l.MixEffectID];
                }

                l.Priority = stream.GetShort(ref index);
                l.HActive = stream.GetShort(ref index);			//HActive of the input itself (used for calculating the scaled border size)
                l.VActive = stream.GetShort(ref index);			//VActive of the input itself (used for Pan calculations)
                l.LayerID = stream.GetShort(ref index) + 2;			//Layer ID (adding offset for Spyder-S zero-based layer ID system)
                _ = stream.GetShort(ref index); //LayerIndex - layer index on frame (we're ignoring this for now)
                l.PixelSpaceID = stream.GetInt(ref index);				//Pixelspace ID
                l.EffectID = stream.GetInt(ref index);
                l.LastScript = stream.GetInt(ref index);
                l.LastCue = stream.GetInt(ref index);

                l.Thumbnail = stream.GetString(ref index);		//thumbnail
                l.WindowLabel = stream.GetString(ref index);
                l.Source = stream.GetString(ref index);
                l.LoadedStill = stream.GetString(ref index);
                l.TestPattern = stream.GetString(ref index);
                l.SourceRouterID = stream.GetInt(ref index);
                l.SourceRouterInput = stream.GetInt(ref index);
                l.InputConfigID = stream.GetInt(ref index);

                _ = stream.GetInt(ref index);  //CurrentInputID - we're ignoring this for now

                l.AspectRatio = stream.GetFloat(ref index);	//aspect ratio
                l.LayerRect = stream.GetRectangle(ref index);
                l.AOIRect = stream.GetRectangle(ref index);

                //Not storing stereoMode, element type
                index++; //StereoMode
                index++; //ElementType

                l.Transparency = stream[index++];				//Visible / Transparency

                //Layer flags (block 1)
                flags = stream[index++];
                l.IsMixer = (flags & 0x01) > 0;
                l.IsFrozen = (flags & 0x02) > 0;
                l.IsSlave = (flags & 0x04) > 0;
                l.AlwaysRelative = (flags & 0x08) > 0;
                kf.BorderOutsideSoftBottom = (flags & 0x10) > 0;
                kf.BorderOutsideSoftLeft = (flags & 0x20) > 0;
                kf.BorderOutsideSoftRight = (flags & 0x40) > 0;
                l.AutoSyncOnTimingChange = (flags & 0x80) > 0;

                //Layer flags (block 2)
                flags = stream[index++];
                kf.BorderOutsideSoftTop = (flags & 0x01) > 0;
                kf.UseDefaultMotionValues = (flags & 0x02) > 0;
                l.IsBackground = (flags & 0x04) > 0;
                l.IsHardwarePreview = (flags & 0x08) > 0;
                l.IsMixing = (flags & 0x10) > 0;
                l.IsWithinPixelSpace = (flags & 0x20) > 0;
                l.HdcpAuthenticated = (flags & 0x40) > 0;
                l.ShadowIsEnabled = (flags & 0x80) > 0;

                //Layer flags (block 3)
                flags = stream[index++];
                l.IsLocked = (flags & 0x01) > 0;
                //l.IsVisible = (flags & 0x02) > 0;
                //l.IsDrawingApplicable = (flags & 0x04) > 0;
                //l.IsDualLayerMode = (flags & 0x08) > 0;
                //l.HasFrameBuffer = (flags & 0x10) > 0;
                //l.InputConnectionDetected = (flags & 0x20) > 0;

                kf.BorderThickness = stream.GetShort(ref index);	//border thickness
                kf.Width = (ushort)stream.GetShort(ref index);			//HSize
                kf.BorderInsideSoftness = stream.GetShort(ref index);	//Border inside softness
                kf.BorderOutsideSoftness = stream.GetShort(ref index);  //Border outside softness

                kf.ShadowHOffset = stream.GetShort(ref index);	//Shadow H Offset
                kf.ShadowVOffset = stream.GetShort(ref index);	//Shadow V Offset
                kf.ShadowHSize = stream.GetShort(ref index);		//Shadow H Size
                kf.ShadowVSize = kf.ShadowHSize;

                kf.ShadowSoftness = stream.GetShort(ref index);	//Shadow softness
                kf.ShadowTransparency = stream.GetShort(ref index);	//Shadow Transparency
                kf.BorderLumaOffsetBottom = stream.GetShort(ref index);
                kf.BorderLumaOffsetLeft = stream.GetShort(ref index);
                kf.BorderLumaOffsetRight = stream.GetShort(ref index);
                kf.BorderLumaOffsetTop = stream.GetShort(ref index);

                kf.HPosition = stream.GetFloat(ref index);			//HPosition
                kf.VPosition = stream.GetFloat(ref index);			//VPosition
                kf.TopCrop = stream.GetFloat(ref index);		//top crop
                kf.LeftCrop = stream.GetFloat(ref index);		//Left crop
                kf.RightCrop = stream.GetFloat(ref index);		//Right crop
                kf.BottomCrop = stream.GetFloat(ref index);	//Bottom crop
                kf.Zoom = stream.GetFloat(ref index);			//Zoom for zoom/pan
                kf.AspectRatioOffset = stream.GetFloat(ref index);	//Aspect ratio offset
                kf.EaseIn = stream.GetFloat(ref index);        //Ease in
                kf.EaseOut = stream.GetFloat(ref index);       //Ease out
                kf.PanH = stream.GetInt(ref index);			//Horizontal Pan
                kf.PanV = stream.GetInt(ref index);			//Vertical Pan
                kf.CropAnchor = (CropAnchorTypes)stream[index++];
                kf.Transparency = stream[index++];      //Transparency
                kf.Duration = stream.GetShort(ref index);
                kf.CloneMode = (CloneMode)stream[index++];		//clone mode

                //Clone offsets
                int cloneOffsetCount = stream[index++];
                float[] cloneOffsets = new float[cloneOffsetCount];
                for(int j=0; j<cloneOffsets.Length; j++)
                {
                    cloneOffsets[j] = stream.GetFloat(ref index);
                }
                kf.CloneOffsets = cloneOffsets;

                //Clone absolute pixel offsets
                int absoluteCloneOffsetCount = stream[index++];
                Rectangle[] cloneRects = new Rectangle[absoluteCloneOffsetCount];
                for (int j = 0; j < cloneRects.Length; j++)
                {
                    int cloneHOffset = stream.GetInt(ref index);
                    cloneRects[j] = new Rectangle()
                    {
                        X = l.LayerRect.X + cloneHOffset,
                        Y = l.LayerRect.Y,
                        Width = l.LayerRect.Width,
                        Height = l.LayerRect.Height
                    };
                }
                l.CloneRects = cloneRects;

                kf.BorderColor = new Color(
                    stream[index++],
                    stream[index++],
                    stream[index++]);

                kf.ShadowColor = new Color(
                    stream[index++],
                    stream[index++],
                    stream[index++]);

                kf.BorderShapeSource = (ShapeSource)stream[index++];
                kf.BorderShape = (ShapeType)stream[index++];
                kf.BorderShapeFile = stream.GetString(ref index);
                kf.BorderShapeStretch = (BorderStretchMode)stream[index++];
                kf.BorderShapeStretchAspectRatio = stream.GetFloat(ref index);

                kf.BorderFillSource = (TextureFillSource)stream[index++];
                kf.BorderTextureType = (TextureType)stream[index++];
                kf.BorderTileMode = (TextureTileMode)stream[index++];
                kf.BorderTextureFile = stream.GetString(ref index);

                //Scale (coerced from parent pixelspace)
                l.Scale = (response.PixelSpaces.ContainsKey(l.PixelSpaceID) ? response.PixelSpaces[l.PixelSpaceID].Scale : 1);

                //Add Keyframe to list
                if (i < newDkCount)
                {
                    response.DrawingKeyFrames.Add(l.LayerID, l);
                }
                else
                {
                    response.PreviewDrawingKeyFrames.Add(l.LayerID, l);
                }
            }

            //Compatibility fix - if any of the PGM layers are not visible but the preview layers are, promote the preview layers to PGM layers
            //This is needed for Spyder X80 V5.0.3 and above, where PGM and PVW layers have become discrete operating objects
            for (int i = 0; i < newDkCount+2; i++)
            {
                var pgmLayer = response.DrawingKeyFrames[i];
                if (pgmLayer != null && !pgmLayer.IsVisible && !pgmLayer.IsBackground)
                {
                    var pvwLayer = response.PreviewDrawingKeyFrames[i];
                    if (pvwLayer.IsVisible)
                    {
                        response.DrawingKeyFrames.Remove(i);
                        response.PreviewDrawingKeyFrames.Remove(i);
                        response.DrawingKeyFrames.Add(i, pvwLayer);
                        response.PreviewDrawingKeyFrames.Add(i, pgmLayer);
                    }
                }
            }

            //Get the routers
            for (int i = 0; i < newRtrCount; i++)
            {
                var router = new DrawingRouter();

                router.Name = stream.GetString(ref index);
                router.TransportType = stream.GetString(ref index);
                router.RouterType = stream.GetString(ref index);
                router.IPAddress = stream.GetString(ref index);

                //Router flags
                flags = stream[index++];
                router.IPRouter = (flags & 0x01) > 0;
                router.SerialRouter = (flags & 0x02) > 0;
                router.LevelControlledRouter = (flags & 0x04) > 0;

                router.ID = stream[index++];
                router.InputCount = stream.GetShort(ref index);
                router.OutputCount = stream.GetShort(ref index);
                router.Port = stream[index++];
                router.ConnectorType = ParseRouterConnectorType(stream[index++]);
                router.ControlLevel = stream.GetInt(ref index);
                router.LevelCount = stream.GetInt(ref index);

                //Write the soft patch for the router
                int patchCount = stream[index++];
                router.Patch.Clear();
                for (int patchIndex = 0; patchIndex < patchCount; patchIndex++)
                {
                    int physicalOutput = stream[index++];
                    int downstreamRouterID = stream[index++];
                    int downstreamRouterInput = stream[index++];

                    //downstream ID may be -1 or -2 if we are patched to a layer.  This will have been clipped to 255
                    if (downstreamRouterID == 255)
                        downstreamRouterID = -1;
                    else if (downstreamRouterID == 254)
                        downstreamRouterID = -2;

                    router.SetRouterOutputPatch(physicalOutput, downstreamRouterID, downstreamRouterInput);
                }

                //Write the crosspoints for the router
                for (int outputID = 0; outputID < router.OutputCount; outputID++)
                {
                    if (router.InputCount >= 255)
                    {
                        //Desrialize two bytes of data
                        router.Crosspoints.Add(stream.GetShort(ref index));
                    }
                    else
                    {
                        //Deserialize only one byte of data
                        int input = stream[index++];
                        if (input == 255)
                            input = -1;

                        router.Crosspoints.Add(input);
                    }
                }

                //Add router to list
                response.Routers.Add(router.ID, router);
            }

            //Get the Machines
            for (int i = 0; i < newMachineCount; i++)
            {
                var newMachine = new DrawingMachine();
                MachineStatus status = newMachine.Status;

                //Port ID
                newMachine.Port = stream[index++];

                //Last PlayItemID
                newMachine.LastPlayItemID = stream.GetInt(ref index);

                //Machine Time
                FieldRate frameRate = (FieldRate)stream[index++];
                long frames = stream.GetLong(ref index);
                newMachine.Time = new TimeCode(frameRate, frames);

                //Machine Status (Block 1)
                flags = stream[index++];
                status.Cued = (flags & 0x01) > 0;
                status.Ejecting = (flags & 0x02) > 0;
                status.FastForwarding = (flags & 0x04) > 0;
                status.Jog = (flags & 0x08) > 0;
                status.Local = (flags & 0x10) > 0;
                status.Playing = (flags & 0x20) > 0;
                status.Recording = (flags & 0x40) > 0;
                status.Rewinding = (flags & 0x80) > 0;

                //Machine Status (Block 2)
                flags = stream[index++];
                status.ServoLock = (flags & 0x01) > 0;
                status.ServoRefMissing = (flags & 0x02) > 0;
                status.Shuttle = (flags & 0x04) > 0;
                status.Standby = (flags & 0x08) > 0;
                status.Still = (flags & 0x10) > 0;
                status.Stopped = (flags & 0x20) > 0;
                status.TapeDir = (flags & 0x40) > 0;
                status.TapeOut = (flags & 0x80) > 0;

                //Machine Status (Block 3)
                flags = stream[index++];
                status.TsoMode = (flags & 0x01) > 0;
                status.Var = (flags & 0x02) > 0;

                //Add to collection
                response.Machines.Add(newMachine.Port, newMachine);
            }

            //Get the outputs
            for (int i = 0; i < newOutputCount; i++)
            {
                DrawingOutput output = new DrawingOutput();

                output.OutputType = OutputModuleType.SpyderS;
                output.ID = stream.GetShort(ref index);
                _ = stream.GetShort(ref index); //PortID - ignoring for now
                output.FrameID = stream.GetShort(ref index);
                output.RenewalMasterFrameID = stream.GetShort(ref index);
                output.HActive = stream.GetShort(ref index);
                output.VActive = stream.GetShort(ref index);
                output.HTotal = stream.GetShort(ref index);
                output.VTotal = stream.GetShort(ref index);

                //Ignoring capabilities for now
                _ = stream.GetShort(ref index);

                byte outputFlags = stream[index++];
                output.Interlaced = (outputFlags & 0x01) > 0;
                output.IsFrameLocked = (outputFlags & 0x02) > 0;
                //output.IsFrozen = (outputFlags & 0x04) > 0; //No IsFrozen flag actually on output yet

                output.VerticalRefresh = stream.GetFloat(ref index);
                output.Name = stream.GetString(ref index);
                output.HdcpStatus = (HdcpLinkStatus)stream[index++];

                output.OutputMode = OutputModeFromByte(stream[index++]);
                output.Rotation = (RotationMode)stream[index++];
                output.MST = (MSTMode)stream[index++];
                output.AuxSource = stream.GetString(ref index);
                output.AuxInput = stream[index++];

                output.UnscaledAuxSourceType = (UnscaledAuxSourceType)stream[index++];
                output.UnscaledAuxSourceId = stream.GetInt(ref index);

                output.ScaledSource = stream.GetRectangle(ref index);
                output.ScaledDest = stream.GetRectangle(ref index);

                int rectCount = stream[index++];
                for (int j = 0; j < rectCount; j++)
                    output.Rectangles.Add(stream.GetRectangle(ref index));

                response.Outputs.Add(output.ID, output);
            }

            //Get Diagnostic status
            for (int i = 0; i < diagnosticWarningCount; i++)
            {
                DiagnosticStatus status = (DiagnosticStatus)stream[index++];
                DiagnosticType diagnosticType = (DiagnosticType)stream[index++];
                response.DiagnosticWarnings.Add(diagnosticType, status);
            }

            //Get the configuration outputs
            response.ConfigOutputs.Clear();
            for (int i = 0; i < configOutputCount; i++)
            {
                response.ConfigOutputs.Add(stream[index++]);
            }

            //[Ignored for now] Get Inputs
            for (int i = 0; i < inputCount; i++)
            {
                _ = stream.GetString(ref index);
            }

            return response;
        }

        protected SpyderModels SpyderModelFromByte(ushort value)
        {
            SpyderSModels model = (SpyderSModels)value;
            return model.Convert();
        }

        protected virtual ConnectorType ParseRouterConnectorType(byte val)
        {
            return val switch
            {
                1 => ConnectorType.HDMI,
                2 => ConnectorType.DisplayPort,
                4 => ConnectorType.SDI,
                _ => ConnectorType.Auto, //Unknown
            };
        }

        protected virtual OutputMode OutputModeFromByte(byte val)
        {
            return val switch
            {
                0 => OutputMode.Normal,
                1 => OutputMode.Multiviewer,
                2 => OutputMode.Scaled,
                3 => OutputMode.Aux,
                4 => OutputMode.UnscaledAux,
                5 => OutputMode.OpMon,
                6 => OutputMode.SourceMon,
                7 => OutputMode.Tiled,
                8 => OutputMode.Unused,
                _ => OutputMode.Normal, //Unknown
            };
        }

        /// <summary>
        /// Someone thought it was a good idea to swap the preview/program enum values in Spyder-S software, so we'll need to flip the values here
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected MixerBus ParseSpyderSMixerBus(byte value)
        {
            return value == 0 ? MixerBus.Program : MixerBus.Preview;

        }

        protected FieldRate ParseSpyderSFieldRate(int value)
        {
            switch (value)
            {
                case 23:
                    return FieldRate.FR_23_98;
                case 24:
                    return FieldRate.FR_24;
                case 25:
                    return FieldRate.FR_25;
                case 29:
                    return FieldRate.FR_29_97;
                case 30:
                    return FieldRate.FR_30;
                case 47:
                    return FieldRate.FR_47_95;
                case 48:
                    return FieldRate.FR_48;
                case 50:
                    return FieldRate.FR_50;
                case 59:
                    return FieldRate.FR_59_94;
                case 60:
                    return FieldRate.FR_60;
                case 96:
                    return FieldRate.FR_96;
                case 100:
                    return FieldRate.FR_100;
                case 119:
                    return FieldRate.FR_119_88;
                case 120:
                    return FieldRate.FR_120;
                default:
                    return FieldRate.Unknown;
            }
        }
    }
}
