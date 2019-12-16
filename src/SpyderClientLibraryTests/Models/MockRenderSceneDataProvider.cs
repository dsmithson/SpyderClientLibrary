using Knightware.Primitives;
using Spyder.Client.Common;
using Spyder.Client.Net.DrawingData;
using System.Threading.Tasks;

namespace Spyder.Client.Models
{
    public class MockRenderSceneDataProvider : IRenderSceneDataProvider
    {
        public Task<InputConfig> GetInputConfig(int inputConfigID)
        {
            return Task.FromResult(new InputConfig()
            {
                ID = inputConfigID,
                HActive = 1920,
                VActive = 1080,
                Name = "Test 1080 Config"
            });
        }

        public Task<Source> GetSource(string sourceName)
        {
            return Task.FromResult(new Source()
            {
                Name = sourceName,
                InputConfigID = 0,
                RouterID = 0,
                RouterInput = 1
            });
        }

        public Task<PixelSpace> GetPixelSpace(int pixelSpaceID)
        {
            return Task.FromResult(new PixelSpace()
            {
                ID = pixelSpaceID,
                Name = "Test PixelSpace",
                Rect = new Rectangle(0, 0, 2560, 1024)
            });
        }

        public Task<Size> GetImageFileDimensions(string imageFileName)
        {
            return Task.FromResult(new Size(1920, 1080));
        }

        public Task<DrawingData> GetDrawingData()
        {
            //TODO: Implement this
            return Task.FromResult<DrawingData>(null);
        }
    }
}
