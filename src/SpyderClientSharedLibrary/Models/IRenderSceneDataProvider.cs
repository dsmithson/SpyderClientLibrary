using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;
using Spyder.Client.Net.DrawingData;
using Knightware.Primitives;

namespace Spyder.Client.Models
{
    public interface IRenderSceneDataProvider
    {
        Task<InputConfig> GetInputConfig(int inputConfigID);
        Task<Source> GetSource(string sourceName);
        Task<PixelSpace> GetPixelSpace(int pixelSpaceID);
        Task<Size> GetImageFileDimensions(string imageFileName);
    }
}
