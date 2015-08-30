using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Net.DrawingData;
using Knightware.Collections;

namespace Spyder.Client.Models
{
    public class AggregateRenderLayerProvider : PropertyChangedBase
    {
        private readonly ObservableCollection<RenderLayer> renderLayers = new ObservableCollection<RenderLayer>();
        public ObservableCollection<RenderLayer> RenderLayers
        {
            get { return renderLayers; }
        }

        public void Update(IEnumerable<RenderLayer> baseLayers, IEnumerable<RenderLayer> overrideLayers)
        {
            var newList = new Dictionary<int, RenderLayer>();

            if (overrideLayers != null)
            {
                foreach (RenderLayer layer in overrideLayers)
                {
                    if (!newList.ContainsKey(layer.LayerID))
                        newList.Add(layer.LayerID, layer);
                }
            }

            if (baseLayers != null)
            {
                foreach (RenderLayer layer in baseLayers)
                {
                    if (!newList.ContainsKey(layer.LayerID))
                        newList.Add(layer.LayerID, layer);
                }
            }

            //Now let's update our existing list from this new list
            newList.Values.CopyTo(this.renderLayers,
                (layer) => layer.LayerID,
                (layer) => new RenderLayer(),
                (source, dest) => dest.CopyFrom(source));
        }
    }
}
