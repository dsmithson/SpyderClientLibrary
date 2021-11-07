using Knightware.Primitives;
using Spyder.Client.Common;
using Spyder.Client.IO;
using System;
using System.Collections.Generic;

namespace Spyder.Client.Net.DrawingData.Deserializers
{
    /// <summary>
    /// Deserializes DrawingData messages in the version 8 (Version 2.10.8) serialization format
    /// </summary>
    public class DrawingDataDeserializer_Version8 : IDrawingDataDeserializer
    {
        public DrawingData Deserialize(byte[] stream)
        {
            if (stream == null || stream.Length == 0)
                return null;

            DrawingData response = new DrawingData();
            int index = 0;

            int newPsCount = stream[index++];
            int newDkCount = stream[index++];
            int newRtrCount = stream[index++];
            int newOutputCount = stream[index++];
            int previewPixelSpaceCount = stream[index++];
            int stereoPixelSpaceCount = stream[index++];
            int newMachineCount = stream[index++];
            int frameCount = stream[index++];
            int frameAORCount = stream[index++];
            int runningScriptCount = stream[index++];
            int diagnosticWarningCount = stream[index++];

            //Global values
            response.TimeOfDay = TimeSpan.Zero; //Not supported in version 8

            response.PercentComplete = stream[index++];
            response.LastFrontPanelMsg = (FrontPanelDisplayCommand)stream[index++];
            response.ProgressString = stream.GetString(ref index);
            response.ConfigSource = stream.GetString(ref index);
            response.ConfigLayer = stream.GetInt(ref index);
            response.ConfigOutput = stream.GetInt(ref index);

            //Global Flags
            byte flags = stream[index++];
            response.OpMonOverlay = (flags & 0x01) > 0;
            response.IsLayerZeroPreviewBackground = (flags & 0x02) > 0; //TODO:  Validate that this is the same as 'IsBackgroundLayerZero'
            response.IsMachineHalEnabled = (flags & 0x04) > 0;
            response.IsRouterHalEnabled = (flags & 0x08) > 0;
            response.IsPreviewOnlyScriptingEnabled = false; //Not supported in version 8
            response.IsStillServerConnected = false; //Not supported in version 8
            response.IsLiveUpdateEnabled = true; //Not supported in version 8

            //System Frame Rate
            response.SystemFrameRate = (FieldRate)stream[index++];

            //Get the frames
            for (int i = 0; i < frameCount; i++)
            {
                int frameID = stream[index++];
                response.Frames.Add(frameID, new DrawingFrame()
                {
                    FrameID = frameID,
                    RenewalMasterFrameID = stream[index++]
                });
            }

            //Get the frame AORs
            for (int i = 0; i < frameAORCount; i++)
            {
                int frameID = stream[index++];
                response.Frames[frameID].FrameAOR = stream.GetRectangle(ref index);
            }

            //Get the running scripts
            for (int i = 0; i < runningScriptCount; i++)
            {
                int scriptID = stream.GetInt(ref index);
                long countDown = stream.GetInt(ref index);
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
            for (int i = 0; i < newDkCount; i++)
            {
                DrawingKeyFrame l = new DrawingKeyFrame();
                KeyFrame kf = l.KeyFrame;

                //Fill Drawing Keyframe
                l.Priority = stream.GetShort(ref index);
                l.HActive = stream.GetShort(ref index);			//HActive of the input itself (used for calculating the scaled border size)
                l.VActive = stream.GetShort(ref index);			//VActive of the input itself (used for Pan calculations)
                l.LayerID = stream.GetShort(ref index);			//Layer ID
                l.IsBackground = (l.LayerID < 2);

                short cloneHOffset = stream.GetShort(ref index);
                l.PixelSpaceID = stream.GetInt(ref index);				//Pixelspace ID
                l.EffectID = stream.GetInt(ref index);
                l.LastScript = stream.GetInt(ref index);
                l.LastCue = stream.GetInt(ref index);
                int renewalMasterFrameID = stream.GetInt(ref index);

                l.Thumbnail = stream.GetString(ref index);		//thumbnail
                l.WindowLabel = stream.GetString(ref index);
                l.Source = stream.GetString(ref index);
                l.AspectRatio = stream.GetFloat(ref index);	//aspect ratio
                l.LayerRect = stream.GetRectangle(ref index);

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

                kf.ShadowColor = new Color(0, 0, 0);    //Not supported in version 8

                kf.CropAnchor = (CropAnchorTypes)stream[index++];
                l.StereoMode = (InputStereoMode)stream[index++];

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

                //AOI (coerced from layer info)
                l.AOIRect = LayerHelpers.GetAOIRectangle(l.KeyFrame, new InputConfig()
                {
                    AspectRatio = l.AspectRatio,
                    HActive = l.HActive,
                    VActive = l.VActive,
                });

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
                router.Crosspoints = new List<int>();
                for (int outputIndex = 0; outputIndex < router.OutputCount; outputIndex++)
                    router.Crosspoints.Add(-1);

                int crosspointsCount = stream[index++];
                for (int crosspointIndex = 0; crosspointIndex < crosspointsCount; crosspointIndex++)
                {
                    int output = stream.GetShort(ref index);
                    int input = stream.GetShort(ref index);
                    router.Crosspoints[output] = input;
                }

                //Add router to list
                response.Routers.Add(router.ID, router);
            }

            //Get the Machines
            for (int i = 0; i < newMachineCount; i++)
            {
                var newMachine = new DrawingMachine();
                MachineStatus status = newMachine.Status;

                long frames = stream.GetLong(ref index);

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

                //Stuff we don't use
                string machineType = stream.GetString(ref index);
                string machineName = stream.GetString(ref index);
                int baud = stream.GetShort(ref index);

                newMachine.LastPlayItemID = stream.GetShort(ref index);
                FieldRate rate = (FieldRate)stream[index++];
                newMachine.Time.Set(rate, frames);
                newMachine.Port = stream[index++];

                //Few more things we don't use
                byte parity = stream[index++];
                byte stopBits = stream[index++];

                //Add to collection
                response.Machines.Add(newMachine.Port, newMachine);
            }

            //Get the outputs
            for (int i = 0; i < newOutputCount; i++)
            {
                DrawingOutput output = new DrawingOutput();

                output.OutputType = (OutputModuleType)stream[index++];

                if (output.OutputType == OutputModuleType.SpyderDX4)
                {
                    output.ID = stream.GetShort(ref index);
                    int outputFrameID = stream.GetShort(ref index); //TODO:  Do I need this?
                    int outputAX = stream.GetShort(ref index);
                    int outputAY = stream.GetShort(ref index);
                    int outputBOffset = stream.GetShort(ref index);
                    int outputCX = stream.GetShort(ref index);
                    int outputCY = stream.GetShort(ref index);
                    int outputDOffset = stream.GetShort(ref index);
                    byte pairModeAB = stream[index++];
                    byte pairModeCD = stream[index++];
                    output.HActive = stream.GetShort(ref index);
                    output.VActive = stream.GetShort(ref index);
                    output.Name = stream.GetString(ref index);
                    output.Rotation = (RotationMode)stream[index++];    //TODO:  Verify this enum maps correctly

                    //TODO:  Number of output rects actually depends on the mode of the DX4...
                    output.OutputMode = OutputMode.Normal;
                    output.Rectangles.Add(new Rectangle(outputAX, outputAY, output.HActive, output.VActive));
                    output.Rectangles.Add(new Rectangle(outputAX + outputBOffset, outputAY, output.HActive, output.VActive));
                    output.Rectangles.Add(new Rectangle(outputCX, outputCY, output.HActive, output.VActive));
                    output.Rectangles.Add(new Rectangle(outputCX + outputDOffset, outputCY, output.HActive, output.VActive));
                }
                else if (output.OutputType == OutputModuleType.SpyderUniversal)
                {
                    output.ID = stream.GetShort(ref index);
                    int outputFrameID = stream.GetShort(ref index); //TODO:  Do I need this?
                    int x = stream.GetInt(ref index);
                    int y = stream.GetShort(ref index);
                    output.HActive = stream.GetShort(ref index);
                    output.VActive = stream.GetShort(ref index);
                    output.Name = stream.GetString(ref index);
                    output.OutputMode = (OutputMode)stream[index++];
                    if (output.OutputMode == OutputMode.Scaled)
                    {
                        output.ScaledSource = stream.GetRectangle(ref index);
                    }
                    else if (output.OutputMode == OutputMode.OpMon)
                    {
                        output.OpMonPreviewSource = stream.GetRectangle(ref index);
                        output.OpMonPreviewDest = stream.GetRectangle(ref index);
                        output.OpMonProgramSource = stream.GetRectangle(ref index);
                        output.OpMonProgramDest = stream.GetRectangle(ref index);
                    }
                    else
                    {
                        output.Rectangles.Add(new Rectangle(x, y, output.HActive, output.VActive));
                    }
                }
                else //Output base (enum value doesn't map correctly though...)
                {
                    output.ID = stream.GetShort(ref index);
                    int outputFrameID = stream.GetShort(ref index); //TODO:  Do I need this?
                    output.HActive = stream.GetShort(ref index);
                    output.VActive = stream.GetShort(ref index);
                    output.Name = stream.GetString(ref index);
                }

                response.Outputs.Add(output.ID, output);
            }

            //Get Diagnostic status
            for (int i = 0; i < diagnosticWarningCount; i++)
            {
                DiagnosticStatus status = (DiagnosticStatus)stream[index++];
                DiagnosticType diagnosticType = (DiagnosticType)stream[index++];
                response.DiagnosticWarnings.Add(diagnosticType, status);
            }

            response.IsDataIOIdle = (stream[index++] == 1);

            return response;
        }

        protected ConnectorType ParseRouterConnectorType(byte val)
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
