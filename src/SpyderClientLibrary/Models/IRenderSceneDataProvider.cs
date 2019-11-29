using Knightware.Primitives;
using Spyder.Client.Common;
using System.Threading.Tasks;

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
