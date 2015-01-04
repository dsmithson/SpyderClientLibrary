using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Net.DrawingData;
using Spyder.Client.Scripting;
using Spyder.Client.Collections;
using Spyder.Client.Common;
using Spyder.Client.Primitives;
using Spyder.Client.Models.StackupProviders;
using Spyder.Client.Net;

namespace Spyder.Client.Models
{
    /// <summary>
    /// Displays a resulting rendering of a specified DrawingData and/or script cue
    /// </summary>
    public class RenderScene : PropertyChangedBase
    {
        private readonly AggregateRenderLayerProvider layerProvider = new AggregateRenderLayerProvider();

        private readonly ObservableCollection<RenderLayer> lastDrawingLayers = new ObservableCollection<RenderLayer>();
        private readonly ObservableCollection<RenderLayer> lastScriptLayers = new ObservableCollection<RenderLayer>();

        private bool collectionChangedInUpdate;
        private readonly ObservableCollection<RenderObject> allRenderSceneObjects;

        private Size displaySize;
        public Size DisplaySize
        {
            get { return displaySize; }
            set
            {
                if (displaySize != value)
                {
                    displaySize = value;
                    OnPropertyChanged();
                }
            }
        }

        public IEnumerable<RenderObject> AllRenderSceneObjects
        {
            get
            {
                return allRenderSceneObjects;
            }
        }

        private IStackupProvider stackupProvider;
        public IStackupProvider StackupProvider
        {
            get { return stackupProvider; }
            set
            {
                if (stackupProvider != value)
                {
                    stackupProvider = value;
                    OnPropertyChanged();
                }
            }
        }

        private IRenderSceneDataProvider renderSceneDataProvider;
        public IRenderSceneDataProvider RenderSceneDataProvider
        {
            get { return renderSceneDataProvider; }
            set
            {
                if (renderSceneDataProvider != value)
                {
                    renderSceneDataProvider = value;
                    OnPropertyChanged();
                }
            }
        }

        private string serverIP;
        public string ServerIP
        {
            get { return serverIP; }
            set
            {
                if (serverIP != value)
                {
                    serverIP = value;
                    OnPropertyChanged();
                }
            }
        }

        public RenderScene(IRenderSceneDataProvider renderSceneDataProvider)
        {
            if (renderSceneDataProvider == null)
                throw new ArgumentException("renderSceneDataProvider cannot be null", "renderSceneDataProvider");

            this.renderSceneDataProvider = renderSceneDataProvider;

            this.allRenderSceneObjects = new ObservableCollection<RenderObject>();
            this.allRenderSceneObjects.CollectionChanged += (sender, args) => collectionChangedInUpdate = true;
        }

        public RenderObject HitTest(Point position)
        {
            if (AllRenderSceneObjects != null)
            {
                foreach (RenderObject renderObject in AllRenderSceneObjects.Reverse())
                {
                    var layer = renderObject as RenderLayer;
                    if (layer != null && (layer.LayerRect.Contains(position) || layer.CloneRect.Contains(position)))
                        return layer;

                    var pixelSpace = renderObject as RenderPixelSpace;
                    if (pixelSpace != null && pixelSpace.Rect.Contains(position))
                        return pixelSpace;
                }
            }
            return null;
        }

        public void Clear()
        {
            allRenderSceneObjects.Clear();
            lastDrawingLayers.Clear();
            lastScriptLayers.Clear();
        }

        public Task Update(DrawingData drawingData)
        {
            return Update(drawingData, null, -1);
        }

        public Task Update(Script script, int scriptCue)
        {
            return Update(null, script, scriptCue);
        }

        public async Task Update(DrawingData drawingData, Script script, int scriptCue)
        {
            collectionChangedInUpdate = false;

            //Get a current list of pixelspaces from our provided data
            IEnumerable<PixelSpace> pixelSpaces = null;
            if (drawingData != null)
            {
                pixelSpaces = drawingData.PixelSpaces.Values;

                //Update our stackup provider before we begin
                if (stackupProvider != null)
                    stackupProvider.UpdateStackupMap(drawingData.PixelSpaces.Values, displaySize);
            }
            else if (script != null)
            {
                pixelSpaces = script.GetElements(scriptCue)
                    .Select(element => element.PixelSpaceID)
                    .Distinct()
                    .Select(id => renderSceneDataProvider.GetPixelSpace(id).Result)
                    .Where(ps => ps != null);

                //Update our stackup provider before we begin
                if (stackupProvider != null)
                    stackupProvider.UpdateStackupMap(pixelSpaces, displaySize);
            }

            //Update render scene pixelspaces first
            if (pixelSpaces != null)
            {
                pixelSpaces.ConstrainedCopyTo(
                        allRenderSceneObjects,
                        (ps) => ps.ID,
                        (ps) => ps.ID,
                        (ps) => new RenderPixelSpace(),
                        (src, dest) => dest.CopyFrom(src, stackupProvider));
            }

            //Update our data
            if (drawingData == null || (script != null && !script.IsRelative))
            {
                lastDrawingLayers.Clear();
            }
            else
            {
                //Update our base layers collection
                drawingData.DrawingKeyFrames
                    .Values
                    .Where(l => !l.IsBackground && l.Transparency < 255 && (stackupProvider == null || !stackupProvider.GenerateOffsetRect(l.LayerRect, l.PixelSpaceID).IsEmpty))
                    .CopyTo(
                    this.lastDrawingLayers,
                    (dkf) => (dkf.LayerID),
                    (layer) => (layer.LayerID),
                    (dkf) => new RenderLayer() { LayerID = dkf.LayerID, PixelSpaceID = dkf.PixelSpaceID },
                    (source, dest) => dest.CopyFrom(source, drawingData.GetPixelSpace(source.PixelSpaceID), stackupProvider));
            }

            if (script != null)
            {
                var layersInScript = new List<int>();
                foreach (var element in script.Elements)
                {
                    for (int i = 0; i < element.LayerCount; i++)
                    {
                        int layerID = element.StartLayer + i;
                        if (!layersInScript.Contains(layerID))
                            layersInScript.Add(layerID);
                    }
                }

                lastScriptLayers.Clear();

                foreach (ScriptElement element in script.GetElements(scriptCue))
                {
                    //Get a collection of render layers represented by this script element
                    int elementCueIndex = scriptCue - element.StartCue + 1;
                    var elementLayers = await RenderLayer.CreateFrom(drawingData, element, elementCueIndex, renderSceneDataProvider, stackupProvider);
                    if (elementLayers != null)
                    {
                        foreach (var elementLayer in elementLayers)
                        {
                            //It's possible for elements to have overlapping layers - most commonly occurring with erroneous slave layer configurations.
                            //We'll check this and make the highest indexed script element 'win'
                            var existing = lastScriptLayers.FirstOrDefault(l => l.LayerID == elementLayer.LayerID);
                            if (existing == null)
                                lastScriptLayers.Add(elementLayer);
                            else
                                existing.CopyFrom(elementLayer);
                        }
                    }
                }
            }

            //Update our layer provider now
            layerProvider.Update(lastDrawingLayers, lastScriptLayers);

            //Update our render scene objects now
            layerProvider.RenderLayers.ConstrainedCopyTo(
                allRenderSceneObjects,
                (layer) => layer.LayerID,
                (layer) => layer.LayerID,
                (layer) => new RenderLayer(layer),
                (src, dst) => dst.CopyFrom(src));

            //Did our collection change?
            if (collectionChangedInUpdate)
            {
                //Force change notification to force the sorted property to be re-evaluated by our UI
                OnPropertyChanged("AllRenderSceneObjects");
            }
        }
    }
}
