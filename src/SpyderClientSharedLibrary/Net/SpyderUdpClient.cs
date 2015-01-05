﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spyder.Client.Common;
using System.Threading.Tasks;
using Spyder.Client.Net;
using System.Diagnostics;
using Spyder.Client.Threading.Tasks;
using Spyder.Client.Diagnostics;
using System.IO;
using Spyder.Client.Text;
using Spyder.Client.Net.Notifications;
using Spyder.Client.IO;
using Spyder.Client.Scripting;
using Spyder.Client.FunctionKeys;
using Spyder.Client.Primitives;
using Spyder.Client.Net.Sockets;

namespace Spyder.Client.Net
{
    public delegate T ResponseParser<T>(IRegister register, List<string> items);

    public class SpyderUdpClient : ISpyderClient
    {
        public const int ServerPort = 11116;

        private readonly Func<IUDPSocket> getUdpSocket;
        
        public bool IsRunning { get; private set; }
        public string ServerIP { get; private set; }

        public SpyderUdpClient(Func<IUDPSocket> getUdpSocket, string serverIP)
        {
            this.getUdpSocket = getUdpSocket;
            this.ServerIP = serverIP;
        }

        public virtual Task<bool> Startup()
        {
            Shutdown();
            this.IsRunning = true;

            //Startup command processor
            retrieveEvent = new AsyncAutoResetEvent();
            immediateCommandQueue = new Queue<Tuple<string, TaskCompletionSource<ServerOperationResult>>>();
            backgroundCommandQueue = new Queue<Tuple<string, TaskCompletionSource<ServerOperationResult>>>();
            RunCommandQueueWorkerAsync();

            return Task.FromResult(true);
        }

        public virtual void Shutdown()
        {
            IsRunning = false;

            //Stop command processor
            while (isCommandProcessorRunning)
            {
                if (retrieveEvent != null)
                    retrieveEvent.Set();
            }
            retrieveEvent = null;
            immediateCommandQueue = new Queue<Tuple<string, TaskCompletionSource<ServerOperationResult>>>();
            backgroundCommandQueue = new Queue<Tuple<string, TaskCompletionSource<ServerOperationResult>>>();
        }

        public virtual Task<Stream> GetImageFileStream(string fileName)
        {
            return GetImageFileStream(fileName, 2048);
        }

        public virtual async Task<Stream> GetImageFileStream(string fileName, int? maxWidthOrHeight)
        {
            ServerOperationResult result;
            if (maxWidthOrHeight == null)
                result = await RetrieveAsync("RIF {0}", EncodeSpyderParameter(fileName));
            else
                result = await RetrieveAsync("RIF {0} {1}", EncodeSpyderParameter(fileName), maxWidthOrHeight.Value);

            if (result.Result != ServerOperationResultCode.Success)
                return null;

            return new MemoryStream(HexUtil.GetBytes(result.ResponseData[0]));
        }

        public virtual async Task<IEnumerable<RegisterPage>> GetRegisterPages(RegisterType type)
        {
            ServerOperationResult result = await RetrieveAsync("RPN {0}", (int)type);
            if (result.Result != ServerOperationResultCode.Success)
                return null;

            int index = 0;
            int count;
            if (!int.TryParse(result.ResponseData[index++], out count))
                return null;

            var response = new List<RegisterPage>();
            for (int i = 0; i < count; i++)
            {
                int pageIndex;
                if (!int.TryParse(result.ResponseData[index++], out pageIndex))
                    return null;

                response.Add(new RegisterPage()
                {
                    PageIndex = pageIndex,
                    Name = result.ResponseData[index++]
                });
            }
            return response;
        }

        public virtual Task<IEnumerable<IRegister>> GetRegisters(RegisterType type)
        {
            return ProcessRRL("RRL {0}", (int)type);
        }

        protected Task<IEnumerable<IRegister>> GetRegisters(RegisterType type, int? pageIndex, int? startIndex, int? maxCountToReturn, int? maxNameLength)
        {
            return ProcessRRL("RRL {0} {1} {2} {3} {4}",
                (int)type,
                (pageIndex.HasValue ? pageIndex.Value : -1),
                (startIndex.HasValue ? startIndex.Value : 0),
                (maxCountToReturn.HasValue ? maxCountToReturn : 100000),
                (maxNameLength.HasValue ? maxNameLength.Value : 1000));
        }

        private async Task<IEnumerable<IRegister>> ProcessRRL(string rrlCommand, params object[] rrlCommandArgs)
        {
            ServerOperationResult result = await RetrieveAsync(rrlCommand, rrlCommandArgs);
            if (result.Result != ServerOperationResultCode.Success)
                return null;

            int index = 0;
            int count;
            if (!int.TryParse(result.ResponseData[index++], out count))
                return null;

            var response = new List<IRegister>();
            for (int i = 0; i < count; i++)
            {
                int registerID;
                if (!int.TryParse(result.ResponseData[index++], out registerID))
                    return null;

                response.Add(new Register()
                {
                    RegisterID = registerID,
                    Name = result.ResponseData[index++]
                });
            }
            return response;
        }
        public virtual async Task<IRegister> GetRegister(RegisterType type, int registerID)
        {
            //Request a single register
            ServerOperationResult result = await RetrieveAsync("RRL {0} -1 {1} 1", type, registerID);
            if (result.Result != ServerOperationResultCode.Success)
                return null;

            return new Register()
            {
                RegisterID = int.Parse(result.ResponseData[0]),
                Name = result.ResponseData[1]
            };
        }

        public virtual Task<IEnumerable<Source>> GetSources()
        {
            return RequestRegisterDetails(RegisterType.Source, GetSource);
        }
        public virtual async Task<Source> GetSource(int sourceRegisterID)
        {
            var register = await GetRegister(RegisterType.Source, sourceRegisterID);
            return await GetSource(register);
        }
        public virtual Task<Source> GetSource(IRegister sourceRegister)
        {
            return RequestRegisterDetails(sourceRegister,
                (responseParts) =>
                {
                    return new Source(sourceRegister)
                    {
                        Name = responseParts[0],
                        RouterID = int.Parse(responseParts[1]),
                        RouterInput = int.Parse(responseParts[2]),
                        InputConfigID = int.Parse(responseParts[3]),
                        PreferredTreatmentID = int.Parse(responseParts[4]),
                        PreferredLayerID = int.Parse(responseParts[5]),
                        //HActive = int.Parse(responseParts[6]),
                        //VActive = int.Parse(responseParts[7]),
                        //FrameRate = float.Parse(responseParts[8])
                    };
                });
        }

        public virtual async Task<Source> GetSource(string sourceName)
        {
            var sources = await GetSources();
            if (sources == null)
                return null;
            else
                return sources.FirstOrDefault(s => string.Compare(sourceName, s.Name, StringComparison.CurrentCultureIgnoreCase) == 0);
        }

        public virtual Task<IEnumerable<CommandKey>> GetCommandKeys()
        {
            return RequestRegisterDetails(RegisterType.CommandKey, GetCommandKey);
        }
        public virtual async Task<CommandKey> GetCommandKey(int commandKeyRegisterID)
        {
            var register = await GetRegister(RegisterType.CommandKey, commandKeyRegisterID);
            return await GetCommandKey(register);
        }
        public virtual Task<CommandKey> GetCommandKey(IRegister commandKeyRegister)
        {
            return RequestRegisterDetails<CommandKey>(commandKeyRegister,
                (responseParts) =>
                {
                    return new CommandKey(commandKeyRegister)
                    {
                        ScriptID = int.Parse(responseParts[0]),
                        IsRelative = responseParts[1] == "1",
                        CueCount = int.Parse(responseParts[2])
                    };
                });
        }

        public virtual Task<IEnumerable<Treatment>> GetTreatments()
        {
            return RequestRegisterDetails(RegisterType.Treatment, GetTreatment);
        }
        public virtual async Task<Treatment> GetTreatment(int treatmentRegisterID)
        {
            var register = await GetRegister(RegisterType.Treatment, treatmentRegisterID);
            return await GetTreatment(register);
        }
        public virtual Task<Treatment> GetTreatment(IRegister treatmentRegister)
        {
            return RequestRegisterDetails<Treatment>(treatmentRegister,
                (responseParts) =>
                {
                    return new Treatment(treatmentRegister)
                    {
                        IsSizeEnabled = responseParts[0] == "1",
                        IsHPositionEnabled = responseParts[1] == "1",
                        IsVPositionEnabled = responseParts[1] == "1",
                        IsBorderEnabled = responseParts[2] == "1",
                        IsShadowEnabled = responseParts[3] == "1",
                        IsCloneEnabled = responseParts[4] == "1",
                        IsCropEnabled = responseParts[5] == "1",
                        IsAspectRatioOffsetEnabled = responseParts[6] == "1",
                        IsPanZoomEnabled = responseParts[7] == "1",
                        IsDurationEnabled = responseParts[8] == "1"
                    };
                });
        }

        public virtual Task<IEnumerable<FunctionKey>> GetFunctionKeys()
        {
            return RequestRegisterDetails(RegisterType.FunctionKey, GetFunctionKey);
        }
        public virtual async Task<FunctionKey> GetFunctionKey(int functionKeyRegisterID)
        {
            var register = await GetRegister(RegisterType.FunctionKey, functionKeyRegisterID);
            return await GetFunctionKey(register);
        }
        public virtual Task<FunctionKey> GetFunctionKey(IRegister functionKeyRegister)
        {
            return RequestRegisterDetails(functionKeyRegister,
                (responseParts) =>
                {
                    FunctionKey response = CreateFunctionKeyFromTypeName(responseParts[1]);
                    response.CopyFrom(functionKeyRegister);
                    response.LookupID = int.Parse(responseParts[0]);
                    response.Name = responseParts[2];

                    return response;
                });
        }

        private FunctionKey CreateFunctionKeyFromTypeName(string functionKeyType)
        {
            switch (functionKeyType)
            {
                case "Source":
                    return new AssignSourceFunctionKey();
                case "Freeze Device(s)":
                    return new FreezeDeviceKey();
                case "Freeze Layer(s)":
                    return new FreezeLayerKey();
                case "Mix BG":
                    return new MixBackgroundFunctionKey();
                case "Network Command":
                    return new NetworkCommandFunctionKey();
                case "Offset":
                    return new LayerPositionOffsetFunctionKey();
                case "Salvo":
                    return new RouterSalvoFunctionKey();
                case "Script Recall":
                    return new ScriptRecallFunctionKey();
                case "Still":
                    return new AssignStillFunctionKey();
                default:
                    return new FunctionKey();
            }
        }

        public virtual Task<IEnumerable<Still>> GetStills()
        {
            return RequestRegisterDetails(RegisterType.Still, GetStill);
        }
        public virtual async Task<Still> GetStill(int stillRegisterID)
        {
            var register = await GetRegister(RegisterType.Still, stillRegisterID);
            return await GetStill(register);
        }
        public virtual Task<Still> GetStill(IRegister stillRegister)
        {
            return RequestRegisterDetails(stillRegister,
                (responseParts) =>
                {
                    return new Still(stillRegister)
                    {
                        FileName = responseParts[0],
                        ImageExistsAtServer = responseParts[1] == "1",
                        Width = int.Parse(responseParts[2]),
                        Height = int.Parse(responseParts[3]),
                        FileSize = long.Parse(responseParts[4])
                    };
                });
        }

        public virtual Task<IEnumerable<PlayItem>> GetPlayItems()
        {
            return RequestRegisterDetails(RegisterType.PlayItem, GetPlayItem);
        }
        public virtual async Task<PlayItem> GetPlayItem(int playItemRegisterID)
        {
            var register = await GetRegister(RegisterType.PlayItem, playItemRegisterID);
            return await GetPlayItem(register);
        }
        public virtual Task<PlayItem> GetPlayItem(IRegister playItemRegister)
        {
            return RequestRegisterDetails(playItemRegister,
                (responseParts) =>
                {
                    return new PlayItem(playItemRegister)
                    {
                        //TODO:  Get info
                    };
                });
        }

        public virtual async Task<bool> DeleteCommandKey(params int[] commandKeyRegisterIDs)
        {
            if (commandKeyRegisterIDs == null || commandKeyRegisterIDs.Length == 0)
                return true;

            bool success = true;
            foreach(int commandKeyRegisterID in commandKeyRegisterIDs)
            {
                var result = await RetrieveAsync("DCK {0} R", commandKeyRegisterID);
                if (result == null || result.Result != ServerOperationResultCode.Success)
                    success = false;
            }
            return success;
        }

        public virtual async Task<int> GetRunningCommandKeyCue(int registerID)
        {
            ServerOperationResult result = await RetrieveAsync("SCR {0} R", registerID);
            if (result.Result != ServerOperationResultCode.Success)
                return -1;

            int cue;
            if (!int.TryParse(result.ResponseData[0], out cue))
                cue = -1;

            return cue;
        }

        public virtual async Task<bool> Save()
        {
            var result = await RetrieveAsync("SAV");
            return (result != null && result.Result == ServerOperationResultCode.Success);
        }

        private async Task<IEnumerable<T>> RequestRegisterDetails<T>(RegisterType type, Func<IRegister, Task<T>> getDetails) where T : class
        {
            List<T> response = new List<T>();
            foreach (IRegister register in await GetRegisters(type))
            {
                response.Add(await getDetails(register));
            }
            return response;
        }
        private async Task<T> RequestRegisterDetails<T>(IRegister register, Func<List<string>, T> parser) where T : class
        {
            if (register == null)
                return null;

            ServerOperationResult serverResponse = await RetrieveAsync("RRD {0} {1}", register.Type, register.RegisterID);
            if (serverResponse == null || serverResponse.Result != ServerOperationResultCode.Success || serverResponse.ResponseData == null)
                return null;

            try
            {
                return parser(serverResponse.ResponseData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        #region Layer Interaction
        
        public async Task<int> RequestLayerCount()
        {
            var result = await RetrieveAsync("RLC");
            int layerCount;
            if (result == null || result.Result != ServerOperationResultCode.Success || result.ResponseData.Count == 0 || !int.TryParse(result.ResponseData[0], out layerCount))
                return -1;
            else
                return layerCount - 2;
        }

        public async Task<int> GetFirstAvailableLayerID()
        {
            int layerCount = await RequestLayerCount();
            if (layerCount <= 0)
                return -1;

            for(int i=0 ; i<layerCount ; i++)
            {
                int layerID = i + 2;
                var result = await RetrieveAsync("RLK {0}", layerID);
                int transparency;
                if (result == null || result.Result != ServerOperationResultCode.Success || result.ResponseData.Count < 33 || !int.TryParse(result.ResponseData[32], out transparency))
                    return -1;

                //We've got transparency - is the layer fully transparent?
                if (transparency == 0)
                    return layerID;
            }

            //No layers available
            return -1;
        }
        
        public async Task<bool> ApplyRegisterToLayer(RegisterType type, int registerID, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            var result = await RetrieveAsync("ARL {0} {1} {2}", (int)type, registerID, BuildLayerIDString(layerIDs));
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        public Task<bool> FreezeLayer(params int[] layerIDs)
        {
            return SetLayerFreeze(true, layerIDs);
        }

        public Task<bool> UnFreezeLayer(params int[] layerIDs)
        {
            return SetLayerFreeze(false, layerIDs);
        }

        private async Task<bool> SetLayerFreeze(bool freeze, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            var result = await RetrieveAsync("FRZ {0} {1}",
                (freeze ? "1" : "0"),
                BuildLayerIDString(layerIDs));

            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        public async Task<bool> MixOffAllLayers(int duration)
        {
            int layerCount = await RequestLayerCount();
            if(layerCount <= 0)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Unable to mix off all layers because call to retrieve layer count failed");
                return false;
            }

            //Build a collection of all layerIDs
            int[] layerIDs = new int[layerCount];
            for(int i=0 ; i<layerCount ; i++)
                layerIDs[i] = i+2;

            return await MixOffLayer(duration, layerIDs);
        }

        public Task<bool> MixOffLayer(int duration, params int[] layerIDs)
        {
            return TransitionLayer(0, duration, layerIDs);
        }

        public Task<bool> MixOnLayer(int duration, params int[] layerIDs)
        {
            return TransitionLayer(1, duration, layerIDs);
        }

        public async Task<bool> MixOnLayer(int pixelSpaceID, Point position, int width, int duration, Register content)
        {
            int layerID = await GetFirstAvailableLayerID();
            if (layerID == -1)
                return false;

            return await MixOnLayer(layerID, pixelSpaceID, position, width, duration, content);
        }

        public async Task<bool> MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, int duration, Register content)
        {
            //Sanity checking
            if (layerID < 2 || pixelSpaceID < 0)
                return false;

            if (duration < 1)
                duration = 1;

            //We'll need to know if the target pixelspace is a program or preview pixelspace, which affects how we bring it on screen.
            //The MixOnLayer call only works correctly in PGM pixelspaces - if you call it with a layer in PVW, it is brought to PGM automatically.
            bool isProgramPixelSpace = await IsPixelSpaceProgram(pixelSpaceID);
            
            //Assign this layer to the target PixelSpace, but don't yet make it visible
            bool showLayerImmediately = !isProgramPixelSpace;
            if(!await LayerAssignPixelSpace(pixelSpaceID, showLayerImmediately, layerID))
                return false;

            //Set layer position and size
            if (!await MoveAndResizeLayer(MoveAndResizeType.AbsolutePositionAndSize, position.X, position.Y, width, layerID))
                return false;

            //Clear crop and border settings to get a default look. 
            if (!await AdjustLayerBorder(layerID, 0))
                return false;

            if (!await ResetLayerCrop(layerID) || !await ResetLayerZoomPan(layerID) || !await AdjustLayerAspectRatio(AspectRatioAdjustmentType.SetKeyFrameAspectRatio, 0, layerID))
                return false;

            //Apply content to this layer
            if (!await ApplyRegisterToLayer(content.Type, content.RegisterID, layerID))
                return false;

            if (!showLayerImmediately)
            {
                //Mix this layer on screen now
                if (!await MixOnLayer(duration, layerID))
                    return false;
            }

            //Done!
            return true;
        }

        protected virtual async Task<bool> IsPixelSpaceProgram(int pixelSpaceID)
        {
            //If the mappings list returned does not contain a preview mapping, or the list is null, return true 
            var mappings = await RequestPixelSpaceMappings();
            if (mappings == null || mappings.Any(m => m.PreviewID == pixelSpaceID))
                return false;
            else
                return true;
        }

        protected virtual async Task<List<PixelSpaceMapping>> RequestPixelSpaceMappings()
        {
            var result = await RetrieveAsync("RPM");
            if (result == null || result.Result != ServerOperationResultCode.Success)
                return null;

            int index = 0;
            int count;
            var parts = result.ResponseData;
            if (!int.TryParse(parts[index++], out count))
                return null;

            var response = new List<PixelSpaceMapping>();
            for(int i=0 ; i<count ; i++)
            {
                
                int pgmID, pvwID;
                double scale;
                if (!int.TryParse(parts[index++], out pgmID) || !int.TryParse(parts[index++], out pvwID) || !double.TryParse(parts[index++], out scale))
                    return null;
                else
                    response.Add(new PixelSpaceMapping(pgmID, pvwID, scale));
            }
            return response;
        }

        public Task<bool> TransitionLayer(int duration, params int[] layerIDs)
        {
            return TransitionLayer(2, duration, layerIDs);
        }

        private async Task<bool> TransitionLayer(int operationCode, int duration, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            var result = await RetrieveAsync("TRN {0} {1} {2}", operationCode, duration, BuildLayerIDString(layerIDs));
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        public async Task<bool> ResizeLayer(LayerResizeType resizeType, int hSize, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            if(resizeType == LayerResizeType.AbsolutePixel)
            {
                var result = await RetrieveAsync("KSZ {0} {1}", hSize, BuildLayerIDString(layerIDs));
                return result != null && result.Result == ServerOperationResultCode.Success;
            }
            else
            {
                //Need to read the layer size before updating to a new value - the native KSZ command doesn't support relative sizing
                bool success = true;
                if (hSize != 0)
                {
                    foreach (int layerID in layerIDs)
                    {
                        int currentHSize;
                        var keyFrameRequest = await RetrieveAsync("RLK {0} N", layerID);
                        if (keyFrameRequest.Result == ServerOperationResultCode.Success && keyFrameRequest.ResponseData.Count >= 5 && int.TryParse(keyFrameRequest.ResponseData[4], out currentHSize))
                        {
                            var result = await RetrieveAsync("KSZ {0} {1}", currentHSize + hSize, BuildLayerIDString(layerIDs));
                            if (result == null || result.Result != ServerOperationResultCode.Success)
                                success = false;
                        }
                        else
                        {
                            success = false;
                        }
                    }
                }
                return success;
            }
        }

        public async Task<bool> ResizeLayer(LayerResizeType resizeType, LayerResizeDirection resizeDirection, double value, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            int resizeTypeCode = (resizeType == LayerResizeType.AbsolutePixel ? 0 : 1);
            int resizeDirectionCode = (resizeDirection == LayerResizeDirection.Horizontal ? 0 : 1);
            var result = await RetrieveAsync("AIR {0} {1} {2} {3}", resizeTypeCode, resizeDirectionCode, value, BuildLayerIDString(layerIDs));
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        public async Task<bool> MoveLayer(LayerMoveType moveType, double hPosition, double vPosition, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            //We need to catch an edge case here.  Absolute and Relative pixel adjustments don't work correctly on the server on a preview
            //PixelSpace, and so we need to convert to using relative coordinates before sending to the server if we are trying to do this
            bool success = true;
            if (moveType == LayerMoveType.AbsolutePixel || moveType == LayerMoveType.RelativePixel)
            {
                var pixelSpaces = await RequestPixelSpaces();
                var layersInPreview = new List<Tuple<LayerKeyFrameInfo, PixelSpace>>();
                foreach (int layerID in layerIDs)
                {
                    var layerInfo = await RequestLayerKeyFrame(layerID);
                    if (layerInfo != null)
                    {
                        var pixelSpace = pixelSpaces.FirstOrDefault(p => p.ID == layerInfo.PixelSpaceID);
                        if (pixelSpace != null && pixelSpace.Scale != 1.0)
                        {
                            //This is a layer in preview, and it's call needs to be converted to use relative coordinates
                            layersInPreview.Add(new Tuple<LayerKeyFrameInfo, PixelSpace>(layerInfo, pixelSpace));
                        }
                    }
                }
                if (layersInPreview.Count > 0)
                {
                    //Batch updates for layers by PixelSpace ID
                    var newMoveType = (moveType == LayerMoveType.AbsolutePixel ? LayerMoveType.AbsoluteRelativeCoordinate : LayerMoveType.RelativeRelativeCoordinate);
                    foreach (var entry in layersInPreview.GroupBy(e => e.Item2.ID))
                    {
                        var entryPixelSpace = entry.First().Item2;
                        var entryLayerIDs = entry.Select(e => e.Item1.LayerID).ToArray();
                        var relativeValues = ConvertFromAbsoluteToRelativeCoordinates(entryPixelSpace, (int)hPosition, (int)vPosition);
                        double relativeX = relativeValues.Item1;
                        double relativeY = relativeValues.Item2;
                        if (relativeValues == null || !await MoveLayer(newMoveType, relativeX, relativeY, entryLayerIDs))
                            success = false;
                    }

                    //Remove the layer IDs we just processed
                    var processedLayerIDs = layersInPreview.Select(l => l.Item1.LayerID).ToList();
                    layerIDs = layerIDs.Where(id => !processedLayerIDs.Contains(id)).ToArray();
                    if (layerIDs.Length == 0)
                    {
                        //If there are no more layer IDs to process, we can return now
                        return success;
                    }
                }
            }

            int moveTypeCode;
            switch (moveType)
            {
                case LayerMoveType.AbsolutePixel:
                    moveTypeCode = 0;
                    break;
                case LayerMoveType.RelativePixel:
                    moveTypeCode = 1;
                    break;
                case LayerMoveType.AbsoluteRelativeCoordinate:
                    moveTypeCode = 2;
                    break;
                case LayerMoveType.RelativeRelativeCoordinate:
                    moveTypeCode = 3;
                    break;
                default:
                    throw new ArgumentException("LayerMoveType provided is invalid", "movetype");
            }

            string hString, vString;
            if(moveType == LayerMoveType.AbsolutePixel || moveType == LayerMoveType.RelativePixel)
            {
                hString = ((int)hPosition).ToString();
                vString = ((int)vPosition).ToString();
            }
            else
            {
                hString = hPosition.ToString("0.0000");
                vString = vPosition.ToString("0.0000");
            }

            var result = await RetrieveAsync("KPS {0} {1} {2} {3}", moveTypeCode, hString, vString, BuildLayerIDString(layerIDs));
            return result != null && result.Result == ServerOperationResultCode.Success && success;
        }

        private async Task<Tuple<double, double>> ConvertFromAbsoluteToRelativeCoordinates(int pixelSpaceID, int x, int y)
        {
            var pixelSpace = await RequestPixelSpace(pixelSpaceID);
            if (pixelSpace == null)
                return null;

            return ConvertFromAbsoluteToRelativeCoordinates(pixelSpace, x, y);
        }

        private Tuple<double, double> ConvertFromAbsoluteToRelativeCoordinates(PixelSpace pixelSpace, int x, int y)
        {
            if (pixelSpace == null)
                return null;

            //return ((position * 2f) / widthOrHeight) - 1f;
            double relativeX = (((double)x * 2) / (pixelSpace.Width / pixelSpace.Scale));// -1f;
            double relativeY = (((double)y * 2) / (pixelSpace.Height / pixelSpace.Scale));// - 1f;
            return new Tuple<double, double>(relativeX, relativeY);
        }

        public async Task<bool> MoveAndResizeLayer(MoveAndResizeType moveType, int hPosition, int vPosition, int hSize, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            //Set size
            bool resizeSuccess = true;
            var sizeMode = moveType == MoveAndResizeType.AbsolutePositionAndSize ? LayerResizeType.AbsolutePixel : LayerResizeType.RelativePixel;
            if (sizeMode == LayerResizeType.AbsolutePixel || hSize != 0)
                resizeSuccess = await ResizeLayer(sizeMode, hSize, layerIDs);

            //Set position
            bool positionSuccess = true;
            var moveMode = moveType == MoveAndResizeType.AbsolutePositionAndSize ? LayerMoveType.AbsolutePixel : LayerMoveType.RelativePixel;
            if(moveMode == LayerMoveType.AbsolutePixel || hPosition != 0 || vPosition != 0)
                positionSuccess = await MoveLayer(moveMode, hPosition, vPosition, layerIDs);

            return resizeSuccess && positionSuccess;

            //NOTE:  The native LSP command doesn't work correctly (as of 4.0.1) - abandoning this and setting the two values independantly
            //int moveTypeCode = (moveType == MoveAndResizeType.AbsolutePositionAndSize ? 0 : 1);
            //var result = await RetrieveAsync("LSP {0} {1} {2} {3} {4}", moveTypeCode, hPosition, vPosition, hSize, BuildLayerIDString(layerIDs));
            //return result != null && result.Result == ServerOperationResultCode.Success;
        }

        protected virtual async Task<LayerKeyFrameInfo> RequestLayerKeyFrame(int layerID)
        {
            var result = await RetrieveAsync("RLK {0} N", layerID);
            if (result == null || result.Result != ServerOperationResultCode.Success)
                return null;

            try
            {
                int index = 0;
                var parts = result.ResponseData;
                var response = new LayerKeyFrameInfo();
                response.LayerID = layerID;
                response.HPosition = double.Parse(parts[index++]);
                response.VPosition = double.Parse(parts[index++]);
                response.Rect = new Rectangle()
                {
                    X = int.Parse(parts[index++]),
                    Y = int.Parse(parts[index++]),
                    Width = int.Parse(parts[index++]),
                    Height = int.Parse(parts[index++])
                };
                response.BorderThickness = int.Parse(parts[index++]);
                response.BorderColor = new Color()
                {
                    R = byte.Parse(parts[index++]),
                    G = byte.Parse(parts[index++]),
                    B = byte.Parse(parts[index++])
                };
                response.BorderHBezel = int.Parse(parts[index++]);
                response.BorderVBezel = int.Parse(parts[index++]);
                response.BorderInsideSoftness = int.Parse(parts[index++]);
                response.BorderOutsideSoftness = int.Parse(parts[index++]);

                byte edges = byte.Parse(parts[index++]);
                response.OutsideSoftTop = (edges & 0x01) > 0;
                response.OutsideSoftBottom = (edges & 0x02) > 0;
                response.OutsideSoftLeft = (edges & 0x04) > 0;
                response.OutsideSoftRight = (edges & 0x08) > 0;

                response.ShadowHOffset = int.Parse(parts[index++]);
                response.ShadowVOffset = int.Parse(parts[index++]);
                response.ShadowHSize = int.Parse(parts[index++]);
                response.ShadowSoftness = int.Parse(parts[index++]);
                response.ShadowTransparency = byte.Parse(parts[index++]);

                var cloneMode = byte.Parse(parts[index++]);
                if (cloneMode == 1)
                    response.Clone = CloneMode.Offset;
                else if (cloneMode == 2)
                    response.Clone = CloneMode.Mirror;
                else
                    response.Clone = CloneMode.Off;

                response.CloneOffset = double.Parse(parts[index++]);

                response.CropLeft = double.Parse(parts[index++]);
                response.CropRight = double.Parse(parts[index++]);
                response.CropTop = double.Parse(parts[index++]);
                response.CropBottom = double.Parse(parts[index++]);

                byte cropAnchor = byte.Parse(parts[index++]);
                if (cropAnchor == 0)
                    response.CropAnchor = CropAnchorTypes.InputCenter;
                else
                    response.CropAnchor = CropAnchorTypes.WindowCenter;

                response.AspectRatioOffset = double.Parse(parts[index++]);
                response.Zoom = double.Parse(parts[index++]);
                response.PanHorizontal = int.Parse(parts[index++]);
                response.PanVertical = int.Parse(parts[index++]);
                response.PixelSpaceID = int.Parse(parts[index++]);

                //Transparency was added later to the external protocol, and as such may or may not be available.
                if (index < (parts.Count - 1))
                    response.Transparency = byte.Parse(parts[index++]);

                return response;
            }
            catch(Exception ex)
            {
                //Failed to parse
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to parse response for RLK command: {1}",
                    ex.GetType().Name, ex.Message);

                return null;
            }
        }

        #region Treatments

        public async Task<bool> LearnTreatment(int treatmentID, int layerID, bool learnPosition, bool learnCrop, bool learnClone, bool learnBorder, bool learnShadow)
        {
            var result = await RetrieveAsync("KTL {0} {1} {2} {3} {4} {5} {6}", -1, layerID,
                (learnPosition ? "1" : "0"),
                (learnCrop ? "1" : "0"),
                (learnClone ? "1" : "0"),
                (learnBorder ? "1" : "0"),
                (learnShadow ? "1" : "0"));

            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        /// <summary>
        /// Sets outside softness on the layer.  When a non-zero value is entered, border settings will be disabled
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="outsideSoftnessThickness">Outside softness in pixels (0-255)</param>
        /// <returns></returns>
        public async Task<bool> AdjustLayerOutsideSoftness(int layerID, int outsideSoftnessThickness)
        {
            var result = await RetrieveAsync("KBD {0} {1}", layerID, 0 - outsideSoftnessThickness);
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        public async Task<bool> AdjustLayerBorderBevel(int hBevel, int vBevel, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            bool success = true;
            foreach (int layerID in layerIDs)
            {
                var layerProperties = await RequestLayerKeyFrame(layerID);
                if (layerProperties != null)
                {
                    if (!await AdjustLayerBorder(layerID, layerProperties.BorderThickness, layerProperties.BorderColor, hBevel, vBevel))
                        success = false;
                }
                else
                {
                    //Unable to obtain properties for this layer.
                    success = false;
                }
            }
            return success;
        }

        public async Task<bool> AdjustLayerBorderColor(Color borderColor, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            bool success = true;
            foreach(int layerID in layerIDs)
            {
                var layerProperties = await RequestLayerKeyFrame(layerID);
                if(layerProperties != null)
                {
                    if (!await AdjustLayerBorder(layerID, layerProperties.BorderThickness, borderColor))
                        success = false;
                }
                else
                {
                    //Unable to obtain properties for this layer.
                    success = false;
                }
            }
            return success;
        }

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        public async Task<bool> AdjustLayerBorder(int layerID, int borderThickness)
        {
            var result = await RetrieveAsync("KBD {0} {1}", layerID, borderThickness);
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        /// <param name="borderColor">Border color</param>
        public async Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor)
        {
            var result = await RetrieveAsync("KBD {0} {1} {2} {3} {4}", layerID, borderThickness, 
                borderColor.R, borderColor.G, borderColor.B);

            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        /// <param name="borderColor">Border color</param>
        /// <param name="hBevel">Luminance offset for left/right border edges (0-255)</param>
        /// <param name="vBevel">Luminance offset for top/bottom border edges (0-255)</param>
        public async Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel)
        {
            var result = await RetrieveAsync("KBD {0} {1} {2} {3} {4} {5} {6}", layerID, borderThickness,
                borderColor.R, borderColor.G, borderColor.B, hBevel, vBevel);

            return result != null && result.Result == ServerOperationResultCode.Success;
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
        public async Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel, int insideSoftness)
        {
            var result = await RetrieveAsync("KBD {0} {1} {2} {3} {4} {5} {6} {7}", layerID, borderThickness,
                borderColor.R, borderColor.G, borderColor.B, hBevel, vBevel, insideSoftness);

            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        public async Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition)
        {
            var result = await RetrieveAsync("KSH {0} {1} {2}", layerID, hPosition, vPosition);
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        /// <param name="size">Width of shadow, added to the width of the layer (0-255)</param>
        public async Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size)
        {
            var result = await RetrieveAsync("KSH {0} {1} {2} {3}", layerID, hPosition, vPosition, size);
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        /// <param name="size">Width of shadow, added to the width of the layer (0-255)</param>
        /// <param name="transparency">Visibility of shadow (0-255)</param>
        public async Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency)
        {
            var result = await RetrieveAsync("KSH {0} {1} {2} {3} {4}", layerID, hPosition, vPosition, size, transparency);
            return result != null && result.Result == ServerOperationResultCode.Success;
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
        public async Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency, int outsideSoftness)
        {
            var result = await RetrieveAsync("KSH {0} {1} {2} {3} {4} {5}", layerID, hPosition, vPosition, size, transparency, outsideSoftness);
            return result != null && result.Result == ServerOperationResultCode.Success;
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
        public async Task<bool> AdjustLayerCrop(double? leftCrop, double? rightCrop, double? topCrop, double? bottomCrop, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            if (leftCrop != null && rightCrop != null && topCrop != null && bottomCrop != null)
            {
                //All parameters present, call the command directly
                var result = await RetrieveAsync("CRP {0:0.00} {1:0.00} {2:0.00} {3:0.00} {4}",
                    leftCrop.Value, rightCrop.Value, topCrop.Value, bottomCrop.Value, BuildLayerIDString(layerIDs));

                return result != null && result.Result == ServerOperationResultCode.Success;
            }
            else
            {
                //Need to lookup the existing crop values for the missing edges before sending the command.  To ensure a fresh pull of the data,
                //we'll get layer properties now
                bool allsuccess = true;
                foreach (int layerID in layerIDs)
                {
                    var result = await RetrieveAsync("RLK {0} N", layerID);
                    if (result == null || result.Result != ServerOperationResultCode.Success || result.ResponseData.Count < 26)
                        return false;

                    if (leftCrop == null)
                        leftCrop = double.Parse(result.ResponseData[22]);
                    if (rightCrop == null)
                        rightCrop = double.Parse(result.ResponseData[23]);
                    if (topCrop == null)
                        topCrop = double.Parse(result.ResponseData[24]);
                    if (bottomCrop == null)
                        bottomCrop = double.Parse(result.ResponseData[25]);

                    result = await RetrieveAsync("CRP {0:0.00} {1:0.00} {2:0.00} {3:0.00} {4}",
                        leftCrop.Value, rightCrop.Value, topCrop.Value, bottomCrop.Value, layerID);

                    if (result == null || result.Result != ServerOperationResultCode.Success)
                        allsuccess = false;
                }
                return allsuccess;
            }
        }

        /// <summary>
        /// Resets the crop on one or more specified layers
        /// </summary>
        /// <param name="layerIDs">Layer ID(s) to reset crop on</param>
        public Task<bool> ResetLayerCrop(params int[] layerIDs)
        {
            return AdjustLayerCrop(0, 0, 0, 0, layerIDs);
        }

        /// <summary>
        /// Applies a zoom/pan adjustment to a specified layer
        /// </summary>
        /// <param name="layerID">Layer ID to adjust</param>
        /// <param name="type">Absolute or Relative setting</param>
        /// <param name="zoom">Zoom value (0.0 to 20.0)</param>
        /// <param name="horizontalPan">Pixel offset value for pan (-2048 to 2048)</param>
        /// <param name="verticalPan">Pixel offset value for pan (-2048 to 2048)</param>
        public async Task<bool> AdjustLayerZoomPan(int layerID, AdjustmentType type, double zoom, int horizontalPan, int verticalPan)
        {
            var result = await RetrieveAsync("ZPA {0} {1:0.00} {2} {3} {4}",
                (type == AdjustmentType.Absolute ? "0" : "1"), zoom, horizontalPan, verticalPan, layerID);

            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        /// <summary>
        /// Resets zoom/pan to default values
        /// </summary>
        /// <param name="layerID">Layer ID to reset zoom/pan values for</param>
        public Task<bool> ResetLayerZoomPan(int layerID)
        {
            return AdjustLayerZoomPan(layerID, AdjustmentType.Absolute, 0, 0, 0);
        }

        public async Task<bool> AdjustLayerAspectRatio(AspectRatioAdjustmentType type, double aspectRatioValue, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            string typeCode;
            switch (type)
            {
                case AspectRatioAdjustmentType.SetLayerAspectRatio:
                    typeCode = "t";
                    break;
                case AspectRatioAdjustmentType.SetKeyFrameAspectRatio:
                    typeCode = "o";
                    break;
                case AspectRatioAdjustmentType.OffsetKeyFrameAspectRatio:
                    typeCode = "a";
                    break;
                default:
                    throw new ArgumentException("Unsupported value provided for 'type' argument", "type");
            }

            var result = await RetrieveAsync("ARO {0} {1:0.00} {2}", typeCode, aspectRatioValue, BuildLayerIDString(layerIDs));
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        /// <summary>
        /// Assigns one or more layers to a pixelspace
        /// </summary>
        /// <param name="pixelSpaceID">PixelSpace ID to assign layer(s) to</param>
        /// <param name="makeLayerVisible">When true, layer(s) will </param>
        /// <param name="layerIDs"></param>
        /// <returns></returns>
        public async Task<bool> LayerAssignPixelSpace(int pixelSpaceID, bool makeLayerVisible, params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
                return true;

            var result = await RetrieveAsync("LAP {0} {1} {2}", 
                pixelSpaceID,
                (makeLayerVisible ? "1" : "0"),
                BuildLayerIDString(layerIDs));

            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        #endregion

        #endregion

        #region PixelSpace Interaction

        public async Task<bool> MixBackground(int duration)
        {
            var result = await RetrieveAsync("BTR {0}", duration);
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        public async Task<bool> LoadBackgroundImage(int pixelSpaceID, BackgroundImageBus bus, string fileName)
        {
            var result = await RetrieveAsync("BLD {0} {1} {2}", EncodeSpyderParameter(fileName), pixelSpaceID, (bus == BackgroundImageBus.CurrentBackground ? 1 : 0));
            return result != null && result.Result == ServerOperationResultCode.Success;
        }

        public virtual async Task<PixelSpace> RequestPixelSpace(int pixelSpaceID)
        {
            var pixelSpaces = await RequestPixelSpaces();
            return pixelSpaces == null ? null : pixelSpaces.FirstOrDefault(p => p.ID == pixelSpaceID);
        }

        public virtual async Task<List<PixelSpace>> RequestPixelSpaces()
        {
            var result = await RetrieveAsync("RPD");
            if (result == null || result.Result != ServerOperationResultCode.Success)
                return null;

            int index = 0;
            int count;
            var parts = result.ResponseData;
            if (!int.TryParse(parts[index++], out count))
                return null;

            //Get PixelSpaceMappings, which will give us pixelspace scale
            var mappings = await RequestPixelSpaceMappings();

            var response = new List<PixelSpace>();
            for(int i=0; i<count; i++)
            {
                int id, xPosition, yPosition, width, height;
                if (!int.TryParse(parts[index++], out id))
                    return null;

                var pixelSpace = new PixelSpace();
                pixelSpace.ID = id;
                pixelSpace.Name = parts[index++];
                pixelSpace.LastBackgroundStill = parts[index++];
                pixelSpace.NextBackgroundStill = parts[index++];

                if (!int.TryParse(parts[index++], out xPosition) || !int.TryParse(parts[index++], out yPosition) || !int.TryParse(parts[index++], out width) || !int.TryParse(parts[index++], out height))
                    return null;

                pixelSpace.Rect = new Rectangle(xPosition, yPosition, width, height);

                //Assign scale if this is a preview pixelspace
                var mapping = (mappings == null ? null : mappings.FirstOrDefault(m => m.PreviewID == id));
                if (mapping != null)
                    pixelSpace.Scale = (float)mapping.Scale;

                response.Add(pixelSpace);
            }
            return response;
        }

        #endregion

        #region Command Key Learn/Recall Logic

        public virtual Task<bool> LearnCommandKey(int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative)
        {
            return LearnCommandKey(0, registerID, name, learnFrom, learnAsMixers, learnAsRelative);
        }

        public virtual async Task<bool> LearnCommandKey(int pageIndex, int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative)
        {
            //Force to first page if no page is specified
            if (pageIndex < 0)
                pageIndex = 0;

            //Sanity check / validate the supplied register ID
            if(registerID.HasValue && registerID.Value >= 0)
            {
                //Enforce the appropriate register offset for the supplied page index
                registerID = (pageIndex * 1000) + (registerID % 1000);
            }
            else
            {
                //We need to perform a lookup to find the first available register ID
                var registers = await GetRegisters(RegisterType.CommandKey, pageIndex, null, null, 1);
                if (registers == null)
                    return false;

                var existingIDs = registers.Select(r => r.RegisterID).ToList();
                int newRegisterID = pageIndex * 1000;
                while(existingIDs.Contains(newRegisterID))
                {
                    newRegisterID++;
                }
                registerID = newRegisterID;
            }

            //Send command to server
            ServerOperationResult response = await RetrieveAsync("LCK {0} {1} {2} {3} {4}",
                (learnAsRelative ? "1" : "0"),
                EncodeSpyderParameter(name),
                registerID.Value,
                (learnFrom == MixerBus.Preview ? "1" : "2"),
                (learnAsMixers ? "1" : "0"));

            return response.Result == ServerOperationResultCode.Success;
        }

        public virtual async Task<bool> RecallCommandKey(int registerID, int cueIndex)
        {
            ServerOperationResult response = await RetrieveAsync("RSC {0} {1} R", registerID, cueIndex);
            return response.Result == ServerOperationResultCode.Success;
        }
        public virtual async Task<bool> RecallFunctionKey(int registerID)
        {
            ServerOperationResult response = await RetrieveAsync("FKR {0} R", registerID);
            return response.Result == ServerOperationResultCode.Success;
        }
        public virtual async Task<bool> RecallRegisterToLayer(RegisterType registerType, int registerID, params int[] layerIDs)
        {
            ServerOperationResult response = await RetrieveAsync("ARL {0} {1} {2}", (int)registerType, registerID, BuildLayerIDString(layerIDs));
            return response.Result == ServerOperationResultCode.Success;
        }

        #endregion

        #region Retrieve Logic

        private bool isCommandProcessorRunning;
        private AsyncAutoResetEvent retrieveEvent;
        private Queue<Tuple<string, TaskCompletionSource<ServerOperationResult>>> immediateCommandQueue;
        private Queue<Tuple<string, TaskCompletionSource<ServerOperationResult>>> backgroundCommandQueue;

        protected Task<ServerOperationResult> RetrieveAsync(string command, params object[] args)
        {
            return RetrieveAsync(false, command, args);
        }

        protected Task<ServerOperationResult> RetrieveInBackgroundAsync(string command, params object[] args)
        {
            return RetrieveAsync(true, command, args);
        }

        private Task<ServerOperationResult> RetrieveAsync(bool isBackgroundTask, string command, params object[] args)
        {
            if (command == null || command.Length == 0)
                return Task.FromResult(new ServerOperationResult(ServerOperationResultCode.MissingCommandData));

            if (!IsRunning)
                return Task.FromResult(new ServerOperationResult(ServerOperationResultCode.ClientNotRunning));

            //Pre-process arguments in command string (if any)
            if (args != null && args.Length > 0)
            {
                command = string.Format(command, args);
            }

            //Enqueue command to be processed
            TaskCompletionSource<ServerOperationResult> tcs = null;
            var targetQueue = (isBackgroundTask ? backgroundCommandQueue : immediateCommandQueue);
            lock (targetQueue)
            {
                //Quick check to first see if this command already exists in the queue
                var existing = targetQueue.FirstOrDefault(item => item.Item1 == command);
                if (existing != null)
                {
                    //Since this message is already enqueued, lets return a handle to it's task and save the extra call
                    return existing.Item2.Task;
                }

                //Enqueue a new command to be processed
                tcs = new TaskCompletionSource<ServerOperationResult>();
                targetQueue.Enqueue(new Tuple<string, TaskCompletionSource<ServerOperationResult>>(command, tcs));
            }

            //Fire our autoreset event and then return the associated task for our new completion source
            retrieveEvent.Set();
            return tcs.Task;
        }

        /// <summary>
        /// Worker that processes the retrieve queue while the client is started
        /// </summary>
        private async void RunCommandQueueWorkerAsync()
        {
            isCommandProcessorRunning = true;

            while (IsRunning)
            {
                //Get next item in the list
                CommandExecutionPriority commandQueue;
                var currentOperation = DequeueNextCommand(out commandQueue);
                if (currentOperation == null)
                {
                    //Wait to be signalled
                    await retrieveEvent.WaitAsync();
                    continue;
                }

                //Process this current command
                string command = currentOperation.Item1;
                TaskCompletionSource<ServerOperationResult> tcs = currentOperation.Item2;
                ServerOperationResult result = await CommandQueueWorkerProcessSingleCommandAsync(command);
                result.ExecutionPriorityLevel = commandQueue;
                tcs.TrySetResult(result);
            }

            isCommandProcessorRunning = false;
        }

        private Tuple<string, TaskCompletionSource<ServerOperationResult>> DequeueNextCommand(out CommandExecutionPriority commandQueue)
        {
            //Look in the immediate queue for an item
            lock (immediateCommandQueue)
            {
                if (immediateCommandQueue.Count > 0)
                {
                    commandQueue = CommandExecutionPriority.Immediate;
                    return immediateCommandQueue.Dequeue();
                }
            }

            //Look in the background queue for an item
            lock (backgroundCommandQueue)
            {
                if (backgroundCommandQueue.Count > 0)
                {
                    commandQueue = CommandExecutionPriority.Background;
                    return backgroundCommandQueue.Dequeue();
                }
            }

            //No items in the queues
            commandQueue = CommandExecutionPriority.Immediate;
            return null;
        }

        private async Task<ServerOperationResult> CommandQueueWorkerProcessSingleCommandAsync(string command)
        {
            IUDPSocket socket = null;
            try
            {
                socket = getUdpSocket();
                if (!await socket.Startup(ServerIP, ServerPort))
                {
                    Debug.WriteLine("Failed to startup socket for Spyder Client");
                    return new ServerOperationResult(ServerOperationResultCode.ExecutionError);
                }

                //Create a full message buffer including the Spyder header
                const int headerLength = 10;
                byte[] fullMessage = new byte[command.Length + headerLength];
                fullMessage[0] = (byte)'s';
                fullMessage[1] = (byte)'p';
                fullMessage[2] = (byte)'y';
                fullMessage[3] = (byte)'d';
                fullMessage[4] = (byte)'e';
                fullMessage[5] = (byte)'r';

                //copy the message contents into the full buffer
                for (int i = 0; i < command.Length; i++)
                    fullMessage[headerLength + i] = (byte)command[i];

                //Send message to server and get response
                byte[] responseData = await socket.RetrieveDataAsync(fullMessage, 0, fullMessage.Length, TimeSpan.FromSeconds(1));
                if (responseData == null || responseData.Length == 0)
                    return new ServerOperationResult(ServerOperationResultCode.NoResponseFromServer);

                //Parse out our response
                List<string> responseParts = ParseResponse(responseData);

                //Server result code should be available as an integer in the first response argument
                int resultCode;
                if (!int.TryParse(responseParts[0], out resultCode))
                    return new ServerOperationResult(ServerOperationResultCode.BadResponseFromServer);

                responseParts.RemoveAt(0);
                return new ServerOperationResult((ServerOperationResultCode)resultCode) { ResponseData = responseParts };
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while processing command: {1}", ex.GetType().Name, ex.Message);
                return new ServerOperationResult(ServerOperationResultCode.ExecutionError);
            }
            finally
            {
                if(socket != null)
                {
                    socket.Shutdown();
                    socket = null;
                }
            }
        }

        private List<string> ParseResponse(byte[] responseData)
        {
            List<string> response = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < responseData.Length; i++)
            {
                char c = (char)responseData[i];
                if (c == ' ')
                {
                    //Separator encountered.  Add current StringBuilder contents to response list and reset state
                    if (builder.Length > 0)
                    {
                        response.Add(builder.ToString());
                        builder.Clear();
                    }
                }
                else
                {
                    //Add character to the current response string
                    builder.Append(c);
                }
            }

            //if we have anything left in the builder's buffer, flush it now
            if (builder.Length > 0)
            {
                response.Add(builder.ToString());
            }

            //Decode any embedded '%20' characters used to represent a space
            for (int i = 0; i < response.Count; i++)
            {
                response[i] = DecodeSpyderParameter(response[i]);
            }

            return response;
        }

        #endregion

        #region Helper Methods

        private string BuildLayerIDString(params int[] layerIDs)
        {
            if (layerIDs == null || layerIDs.Length == 0)
            {
                return string.Empty;
            }
            else if (layerIDs.Length == 1)
            {
                return layerIDs[0].ToString();
            }
            else
            {
                StringBuilder builder = new StringBuilder(layerIDs.Length * 2);
                for (int i = 0; i < layerIDs.Length; i++)
                {
                    builder.Append(layerIDs[i]);

                    if (i < (layerIDs.Length - 1))
                        builder.Append(' ');
                }
                return builder.ToString();
            }
        }

        private string EncodeSpyderParameter(object parameter)
        {
            if (parameter == null)
                return null;

            string stringToEncode = parameter.ToString();
            if (string.IsNullOrEmpty(stringToEncode) || !stringToEncode.Contains(" "))
                return stringToEncode;
            else
                return stringToEncode.Replace(" ", "%20");
        }

        private string DecodeSpyderParameter(string encodedString)
        {
            if (string.IsNullOrEmpty(encodedString) || !encodedString.Contains("%20"))
                return encodedString;
            else
                return encodedString.Replace("%20", " ");
        }

        #endregion
    }
}