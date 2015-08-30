﻿using Spyder.Client.Common;
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

namespace Spyder.Client.Net
{
    public interface ISpyderClient
    {
        bool IsRunning { get; }
        string ServerIP { get; }

        Task<bool> Startup();
        void Shutdown();

        #region Server Filesystem Access

        Task<Stream> GetImageFileStream(string fileName);

        #endregion

        #region Register List Data Access

        Task<IEnumerable<RegisterPage>> GetRegisterPages(RegisterType type);
        Task<IEnumerable<IRegister>> GetRegisters(RegisterType type);
        Task<IRegister> GetRegister(RegisterType type, int registerID);

        Task<IEnumerable<Source>> GetSources();
        Task<Source> GetSource(int sourceRegisterID);
        Task<Source> GetSource(IRegister sourceRegister);
        Task<Source> GetSource(string sourceName);

        Task<IEnumerable<CommandKey>> GetCommandKeys();
        Task<CommandKey> GetCommandKey(int commandKeyRegisterID);
        Task<CommandKey> GetCommandKey(IRegister commandKeyRegister);

        Task<IEnumerable<Treatment>> GetTreatments();
        Task<Treatment> GetTreatment(int treatmentRegisterID);
        Task<Treatment> GetTreatment(IRegister treatmentRegister);

        Task<IEnumerable<FunctionKey>> GetFunctionKeys();
        Task<FunctionKey> GetFunctionKey(int functionKeyRegisterID);
        Task<FunctionKey> GetFunctionKey(IRegister functionKeyRegister);

        Task<IEnumerable<Still>> GetStills();
        Task<Still> GetStill(int stillRegisterID);
        Task<Still> GetStill(IRegister stillRegister);

        Task<IEnumerable<PlayItem>> GetPlayItems();
        Task<PlayItem> GetPlayItem(int playItemRegisterID);
        Task<PlayItem> GetPlayItem(IRegister playItemRegister);

        #endregion

        #region Layer Interaction
        
        Task<int> RequestLayerCount();

        Task<int> GetFirstAvailableLayerID();
                
        Task<bool> ApplyRegisterToLayer(RegisterType type, int registerID, params int[] layerIDs);

        Task<bool> FreezeLayer(params int[] layerIDs);

        Task<bool> UnFreezeLayer(params int[] layerIDs);

        Task<bool> MixOffAllLayers(int duration);

        Task<bool> MixOffLayer(int duration, params int[] layerIDs);

        Task<bool> MixOnLayer(int duration, params int[] layerIDs);

        Task<bool> MixOnLayer(int pixelSpaceID, Point position, int width, int duration, Register content);

        Task<bool> MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, int duration, Register content);

        Task<bool> TransitionLayer(int duration, params int[] layerIDs);

        Task<bool> ResizeLayer(LayerResizeType resizeType, int hSize, params int[] layerIDs);

        Task<bool> ResizeLayer(LayerResizeType resizeType, LayerResizeDirection resizeDirection, double value, params int[] layerIDs);

        Task<bool> MoveLayer(LayerMoveType moveType, double hPosition, double vPosition, params int[] layerIDs);

        Task<bool> MoveAndResizeLayer(MoveAndResizeType moveType, int hPosition, int vPosition, int hSize, params int[] layerIDs);

        #region Treatments

        Task<bool> LearnTreatment(int treatmentID, int layerID, bool learnPosition, bool learnCrop, bool learnClone, bool learnBorder, bool learnShadow);

        /// <summary>
        /// Sets outside softness on the layer.  When a non-zero value is entered, border settings will be disabled
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="outsideSoftnessThickness">Outside softness in pixels (0-255)</param>
        /// <returns></returns>
        Task<bool> AdjustLayerOutsideSoftness(int layerID, int outsideSoftnessThickness);

        Task<bool> AdjustLayerBorderColor(Color borderColor, params int[] layerIDs);

        Task<bool> AdjustLayerBorderBevel(int hBevel, int vBevel, params int[] layerIDs);

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        Task<bool> AdjustLayerBorder(int layerID, int borderThickness);

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        /// <param name="borderColor">Border color</param>
        Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor);

        /// <summary>
        /// Adjusts the border properties of a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="borderThickness">Width of border in pixels (0-255)</param>
        /// <param name="borderColor">Border color</param>
        /// <param name="hBevel">Luminance offset for left/right border edges (0-255)</param>
        /// <param name="vBevel">Luminance offset for top/bottom border edges (0-255)</param>
        Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel);

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
        Task<bool> AdjustLayerBorder(int layerID, int borderThickness, Color borderColor, int hBevel, int vBevel, int insideSoftness);

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition);

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        /// <param name="size">Width of shadow, added to the width of the layer (0-255)</param>
        Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size);

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        /// <param name="size">Width of shadow, added to the width of the layer (0-255)</param>
        /// <param name="transparency">Visibility of shadow (0-255)</param>
        Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency);

        /// <summary>
        /// Adjusts shadow properties on a layer
        /// </summary>
        /// <param name="layerID">LayerID to adjust</param>
        /// <param name="hPosition">Horizontal offset position of shadow (0-255)</param>
        /// <param name="vPosition">Vertical offset position of shadow (0-255)</param>
        /// <param name="size">Width of shadow, added to the width of the layer (0-255)</param>
        /// <param name="transparency">Visibility of shadow (0-255)</param>
        /// <param name="outsideSoftness">Number of pixels for the outside softness blend (0-255)</param>
        Task<bool> AdjustLayerShadow(int layerID, int hPosition, int vPosition, int size, int transparency, int outsideSoftness);

        /// <summary>
        /// Applies crop parameters to one or more specified layers
        /// </summary>
        /// <param name="leftCrop">Percentage of left crop to set on the layer (0.0 to 1.0)</param>
        /// <param name="rightCrop">Percentage of right crop to set on the layer (0.0 to 1.0)</param>
        /// <param name="topCrop">Percentage of top crop to set on the layer (0.0 to 1.0)</param>
        /// <param name="bottomCrop">Percentage of bottom crop to set on the layer (0.0 to 1.0)</param>
        /// <param name="layerIDs">Layer ID(s) to apply crop settings to</param>
        /// <returns></returns>
        Task<bool> AdjustLayerCrop(double? leftCrop, double? rightCrop, double? topCrop, double? bottomCrop, params int[] layerIDs);

        /// <summary>
        /// Resets the crop on one or more specified layers
        /// </summary>
        /// <param name="layerIDs">Layer ID(s) to reset crop on</param>
        Task<bool> ResetLayerCrop(params int[] layerIDs);

        /// <summary>
        /// Applies a zoom/pan adjustment to a specified layer
        /// </summary>
        /// <param name="layerID">Layer ID to adjust</param>
        /// <param name="type">Absolute or Relative setting</param>
        /// <param name="zoom">Zoom value (0.0 to 20.0)</param>
        /// <param name="horizontalPan">Pixel offset value for pan (-2048 to 2048)</param>
        /// <param name="verticalPan">Pixel offset value for pan (-2048 to 2048)</param>
        Task<bool> AdjustLayerZoomPan(int layerID, AdjustmentType type, double zoom, int horizontalPan, int verticalPan);

        /// <summary>
        /// Resets zoom/pan to default values
        /// </summary>
        /// <param name="layerID">Layer ID to reset zoom/pan values for</param>
        Task<bool> ResetLayerZoomPan(int layerID);

        Task<bool> AdjustLayerAspectRatio(AspectRatioAdjustmentType type, double aspectRatioValue, params int[] layerIDs);

        /// <summary>
        /// Assigns one or more layers to a pixelspace
        /// </summary>
        /// <param name="pixelSpaceID">PixelSpace ID to assign layer(s) to</param>
        /// <param name="makeLayerVisible">When true, layer(s) will </param>
        /// <param name="layerIDs"></param>
        /// <returns></returns>
        Task<bool> LayerAssignPixelSpace(int pixelSpaceID, bool makeLayerVisible, params int[] layerIDs);
        
        #endregion

        #endregion

        #region PixelSpace Interaction

        Task<bool> MixBackground(int duration);

        Task<bool> LoadBackgroundImage(int pixelSpaceID, BackgroundImageBus bus, string fileName);

        Task<List<PixelSpace>> RequestPixelSpaces();

        Task<PixelSpace> RequestPixelSpace(int pixelSpaceID);

        #endregion

        #region Script Access

        /// <summary>
        /// Gets the script cue currently running, or -1 if the script is not currently running on any layers
        /// </summary>
        Task<int> GetRunningCommandKeyCue(int registerID);

        Task<bool> DeleteCommandKey(params int[] commandKeyRegisterIDs);

        #endregion

        Task<bool> Save();

        Task<bool> LearnCommandKey(int pageIndex, int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative);

        Task<bool> LearnCommandKey(int? registerID, string name, MixerBus learnFrom, bool learnAsMixers, bool learnAsRelative);

        Task<bool> RecallCommandKey(int registerID, int cueIndex);
        Task<bool> RecallFunctionKey(int registerID);
        Task<bool> RecallRegisterToLayer(RegisterType registerType, int registerID, params int[] layerIDs);
    }
}
