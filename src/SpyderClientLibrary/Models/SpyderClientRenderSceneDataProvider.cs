using Knightware.Primitives;
using Spyder.Client.Common;
using Spyder.Client.Net;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Spyder.Client.Models
{
    /// <summary>
    /// Class that provides Spyder system data used for generating a RenderScene
    /// </summary>
    public class SpyderClientRenderSceneDataProvider : IRenderSceneDataProvider
    {
        protected BindableSpyderClient client;

        public SpyderClientRenderSceneDataProvider(BindableSpyderClient client)
        {
            if (client == null)
                throw new ArgumentException("client cannot be null", "client");

            this.client = client;
        }

        public virtual Task<InputConfig> GetInputConfig(int inputConfigID)
        {
            return client.GetInputConfig(inputConfigID);
        }

        public virtual Task<Source> GetSource(string sourceName)
        {
            return client.GetSource(sourceName);
        }

        public virtual Task<PixelSpace> GetPixelSpace(int pixelSpaceID)
        {
            return client.GetPixelSpace(pixelSpaceID);
        }

        public virtual Task<Size> GetImageFileDimensions(string imageFileName)
        {
            //TODO:  Figure out how to return this info efficiently
            var drawingData = client.DrawingData;
            if (drawingData != null)
            {
                var dkf = drawingData.DrawingKeyFrames.Values.Concat(drawingData.PreviewDrawingKeyFrames.Values)
                    .FirstOrDefault(l => string.Compare(l.LoadedStill, imageFileName, StringComparison.CurrentCultureIgnoreCase) == 0);

                if (dkf != null)
                    return Task.FromResult(new Size(dkf.HActive, dkf.VActive));
            }
            return Task.FromResult(Size.Empty);
        }
    }
}
