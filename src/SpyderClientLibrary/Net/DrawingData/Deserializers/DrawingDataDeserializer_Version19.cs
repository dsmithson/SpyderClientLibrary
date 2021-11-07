using Knightware.Primitives;
using Spyder.Client.Common;
using Spyder.Client.IO;
using System;

namespace Spyder.Client.Net.DrawingData.Deserializers
{
    /// <summary>
    /// Deserializes DrawingData messages in the version 19 serialization format
    /// </summary>
    public class DrawingDataDeserializer_Version19 : IDrawingDataDeserializer
    {
        public DrawingData Deserialize(byte[] stream)
        {
            if (stream == null || stream.Length == 0)
                return null;

            DrawingData response = new DrawingData();
            int index = 0;

            int newPsCount = stream[index++];
            int newDkCount = stream[index++];
            int newPvwDkCount = stream[index++];
            int newRtrCount = stream[index++];
            int newOutputCount = stream[index++];
            int previewPixelSpaceCount = stream[index++];
            int stereoPixelSpaceCount = stream[index++];
            int newMachineCount = stream[index++];
            int frameCount = stream[index++];
            int runningScriptCount = stream[index++];
            int diagnosticWarningCount = stream[index++];

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
            response.ConfigOutput = stream.GetInt(ref index);

            response.HardwareType = (HardwareType)stream[index++];
            response.DataObjectVersion = stream.GetInt(ref index);
            response.DataObjectLastChangeType = (DataType)stream.GetInt(ref index);

            //Global Flags
            byte flags = stream[index++];
            response.OpMonOverlay = (flags & 0x01) > 0;
            response.IsLayerZeroPreviewBackground = (flags & 0x02) > 0;
            response.IsMachineHalEnabled = (flags & 0x04) > 0;
            response.IsRouterHalEnabled = (flags & 0x08) > 0;
            response.IsDataIOIdle = (flags & 0x10) > 0;
            response.IsPreviewOnlyScriptingEnabled = (flags & 0x20) > 0;
            response.IsStillServerConnected = (flags & 0x40) > 0;
            response.IsLiveUpdateEnabled = (flags & 0x80) > 0;

            //System Frame Rate
            response.SystemFrameRate = (FieldRate)stream[index++];

            //Get the frames
            for (int i = 0; i < frameCount; i++)
            {
                int frameID = stream[index++];
                response.Frames.Add(frameID, new DrawingFrame()
                {
                    FrameID = frameID,
                    FrameAOR = stream.GetRectangle(ref index),
                    ProgramAOR = stream.GetRectangle(ref index),
                    PreviewAOR = stream.GetRectangle(ref index),
                    Model = (SpyderModels)stream[index++],
                    RenewalMasterFrameID = stream[index++]
                });
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
                newPS.Scale = stream.GetFloat(ref index);
                newPS.RenewMasterFrameID = stream.GetInt(ref index);
                newPS.ID = stream.GetShort(ref index);
                newPS.Name = stream.GetString(ref index);
                newPS.NextBackgroundStillIsOnLayer1 = !response.IsLayerZeroPreviewBackground;
                newPS.LastBackgroundStill = stream.GetString(ref index);	//Last background still
                newPS.NextBackgroundStill = stream.GetString(ref index);	//Next Background still

                //Add Pixelspace to the list
                response.PixelSpaces.Add(newPS.ID, newPS);
            }

            //Get all of our DrawingKeyFrames
            int allLayerCount = newDkCount + newPvwDkCount;

            for (int i = 0; i < allLayerCount; i++)
            {
                DrawingKeyFrame l = new DrawingKeyFrame();
                KeyFrame kf = l.KeyFrame;

                //Fill Drawing Keyframe
                l.Priority = stream.GetShort(ref index);
                l.HActive = stream.GetShort(ref index);			//HActive of the input itself (used for calculating the scaled border size)
                l.VActive = stream.GetShort(ref index);			//VActive of the input itself (used for Pan calculations)
                l.LayerID = stream.GetShort(ref index);			//Layer ID
                short cloneHOffset = stream.GetShort(ref index);
                l.PixelSpaceID = stream.GetInt(ref index);				//Pixelspace ID
                l.EffectID = stream.GetInt(ref index);
                l.LastScript = stream.GetInt(ref index);
                l.LastCue = stream.GetInt(ref index);

                l.Thumbnail = stream.GetString(ref index);		//thumbnail
                l.WindowLabel = stream.GetString(ref index);
                l.Source = stream.GetString(ref index);
                l.LoadedStill = stream.GetString(ref index);
                l.SourceRouterID = stream.GetInt(ref index);
                l.SourceRouterInput = stream.GetInt(ref index);
                l.InputConfigID = stream.GetInt(ref index);

                l.LinearKeySource = stream.GetString(ref index);
                l.LinearKeyRouterID = stream.GetInt(ref index);
                l.LinearKeyRouterInput = stream.GetInt(ref index);

                l.AspectRatio = stream.GetFloat(ref index);	//aspect ratio
                l.LayerRect = stream.GetRectangle(ref index);
                l.AOIRect = stream.GetRectangle(ref index);

                //Not storing element type
                //l.ElementType = (ElementType)stream[index++];	//Element Type
                index++;

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

                kf.BorderThickness = stream.GetShort(ref index);	//border thickness
                kf.Width = stream.GetShort(ref index);			//HSize
                kf.BorderInsideSoftness = stream.GetShort(ref index);	//Border inside softness
                kf.BorderOutsideSoftness = stream.GetShort(ref index);	//Border outside softness
                kf.ShadowHOffset = stream.GetShort(ref index);	//Shadow H Offset
                kf.ShadowVOffset = stream.GetShort(ref index);	//Shadow V Offset
                kf.ShadowHSize = stream.GetShort(ref index);		//Shadow H Size
                kf.ShadowVSize = stream.GetShort(ref index);		//Shadow V Size
                kf.ShadowSoftness = stream.GetShort(ref index);	//Shadow softness
                kf.ShadowTransparency = stream.GetShort(ref index);	//Shadow Transparency
                kf.BorderLumaOffsetBottom = stream.GetShort(ref index);
                kf.BorderLumaOffsetLeft = stream.GetShort(ref index);
                kf.BorderLumaOffsetRight = stream.GetShort(ref index);
                kf.BorderLumaOffsetTop = stream.GetShort(ref index);

                kf.CloneOffset = stream.GetFloat(ref index);	//clone offset
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
                kf.Duration = stream.GetShort(ref index);
                kf.CloneMode = (CloneMode)stream[index++];		//clone mode
                kf.BorderColor = new Color(
                    stream[index++],
                    stream[index++],
                    stream[index++]);

                kf.ShadowColor = new Color(
                    stream[index++],
                    stream[index++],
                    stream[index++]);

                kf.CropAnchor = (CropAnchorTypes)stream[index++];
                l.StereoMode = (InputStereoMode)stream[index++];

                //Frame Config
                l.FrameID = stream[index++];

                kf.BorderShapeSource = (ShapeSource)stream[index++];
                kf.BorderShape = (ShapeType)stream[index++];
                kf.BorderShapeFile = stream.GetString(ref index);

                kf.BorderFillSource = (TextureFillSource)stream[index++];
                kf.BorderTextureType = (TextureType)stream[index++];
                kf.BorderTileMode = (TextureTileMode)stream[index++];
                kf.BorderTextureFile = stream.GetString(ref index);

                //Calculated values
                l.CloneRect = new Rectangle()
                {
                    X = l.LayerRect.X + cloneHOffset,
                    Y = l.LayerRect.Y,
                    Width = l.LayerRect.Width,
                    Height = l.LayerRect.Height
                };

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

            //Now that we have Pixelspaces and layers, update pixelspace background transparency info from layers
            if (response.DrawingKeyFrames.Count >= 2)
            {
                byte layer1Transparency = response.DrawingKeyFrames[1].Transparency;
                foreach (DrawingPixelSpace ps in response.PixelSpaces.Values)
                {
                    ps.Layer1Transparency = layer1Transparency;
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
                newMachine.Time.Set(frameRate, frames);

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

                output.OutputType = (OutputModuleType)stream[index++];
                output.ID = stream.GetShort(ref index);
                output.FrameID = stream.GetShort(ref index);
                output.RenewalMasterFrameID = stream.GetShort(ref index);
                output.HActive = stream.GetShort(ref index);
                output.VActive = stream.GetShort(ref index);
                output.HTotal = stream.GetShort(ref index);
                output.VTotal = stream.GetShort(ref index);
                output.Interlaced = stream[index++] == 0x01;
                output.VerticalRefresh = stream.GetFloat(ref index);
                output.Name = stream.GetString(ref index);
                output.HdcpStatus = (HdcpLinkStatus)stream[index++];

                output.OutputMode = (OutputMode)stream[index++];
                output.Rotation = (RotationMode)stream[index++];

                output.OpMonProgramSource = stream.GetRectangle(ref index);
                output.OpMonProgramDest = stream.GetRectangle(ref index);
                output.OpMonPreviewSource = stream.GetRectangle(ref index);
                output.OpMonPreviewDest = stream.GetRectangle(ref index);
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

            return response;
        }

        private ConnectorType ParseRouterConnectorType(byte val)
        {
            switch (val)
            {
                case 0: return ConnectorType.Analog;
                case 1: return ConnectorType.DVI;
                case 2: return ConnectorType.SDI;
                case 3: return ConnectorType.SDI;
                case 4: return ConnectorType.Composite;
                case 5: return ConnectorType.SVideo;
                default: throw new ArgumentException($"Unable to convert value {val} to a DrawingData router connector type", nameof(val));
            }
        }
    }
}
