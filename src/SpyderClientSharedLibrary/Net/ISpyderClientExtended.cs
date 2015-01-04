using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;
using Spyder.Client.Net.Notifications;
using Spyder.Client.Scripting;
using Spyder.Client.Primitives;

namespace Spyder.Client.Net
{
    /// <summary>
    /// Extends the basic functionality in the ISpyderClient; adds functionality available in native interfaces to the Spyder server
    /// </summary>
    public interface ISpyderClientExtended : ISpyderClient
    {
        event DrawingDataReceivedHandler DrawingDataReceived;

        event TraceLogMessageHandler TraceLogMessageReceived;

        Task<VersionInfo> GetVersionInfo();

        Task<List<Shape>> GetShapes();
        Task<Shape> GetShape(string shapeFileName);

        Task<List<string>> GetShapeFileNames();

        Task<ServerSettings> GetServerSettings();

        Task<IEnumerable<Script>> GetScripts();
        Task<Script> GetScript(int scriptID);

        Task<IEnumerable<PixelSpace>> GetPixelSpaces();
        Task<PixelSpace> GetPixelSpace(int PixelSpaceID);

        Task<IEnumerable<InputConfig>> GetInputConfigs();
        Task<InputConfig> GetInputConfig(int InputConfigID);
        Task<InputConfig> GetInputConfig(string sourceName);

        Task<bool> MixBackground(TimeSpan duration);

        #region Layer Interactions

        Task<bool> MixOffAllLayers(TimeSpan duration);

        Task<bool> MixOffLayer(TimeSpan duration, params int[] layerIDs);

        Task<bool> MixOnLayer(TimeSpan duration, params int[] layerIDs);

        Task<bool> MixOnLayer(int pixelSpaceID, Point position, int width, TimeSpan duration, Register content);

        Task<bool> MixOnLayer(int layerID, int pixelSpaceID, Point position, int width, TimeSpan duration, Register content);

        Task<bool> TransitionLayer(TimeSpan duration, params int[] layerIDs);

        #endregion
    }
}
