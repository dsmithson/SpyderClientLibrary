using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;
using Spyder.Client.Net.Notifications;
using Spyder.Client.Scripting;
using Knightware.Primitives;
using System.IO;

namespace Spyder.Client.Net
{
    /// <summary>
    /// Extends the basic functionality in the ISpyderClient; adds functionality available in native interfaces to the Spyder server
    /// </summary>
    public interface ISpyderClientExtended : ISpyderClient
    {
        event DrawingDataReceivedHandler DrawingDataReceived;

        event TraceLogMessageHandler TraceLogMessageReceived;

        event DataObjectChangedHandler DataObjectChanged;

        HardwareType HardwareType { get; }

        /// <summary>
        /// Defines a throttle for maximum drawing data event raising (per Spyder server).  Setting to 1 second, for example, will ensure DrawingData does not fire more than once per second.  Set to TimeSpan.Zero (default) to disable throttling.
        /// </summary>
        TimeSpan DrawingDataThrottleInterval { get; set; }

        Task<VersionInfo> GetVersionInfo();

        Task<bool> SetImageFileStream(string fileName, Stream fileStream);

        Task<List<Shape>> GetShapes();
        Task<Shape> GetShape(string shapeFileName);
        Task<bool> SetShape(Shape shape);

        Task<List<string>> GetShapeFileNames();

        Task<ServerSettings> GetServerSettings();

        Task<List<Script>> GetScripts();
        Task<Script> GetScript(int scriptID);
        
        Task<List<InputConfig>> GetInputConfigs();
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
