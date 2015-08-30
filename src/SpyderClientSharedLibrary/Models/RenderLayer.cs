using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Common;
using Spyder.Client.Models.StackupProviders;
using Spyder.Client.Net.DrawingData;
using Knightware.Primitives;
using Spyder.Client.Scripting;

namespace Spyder.Client.Models
{
    public enum RenderObjectSource { Undefined, DrawingData, Script }

    public class RenderLayer : RenderObject
    {
        private int layerID;
        public int LayerID
        {
            get { return layerID; }
            set
            {
                if (layerID != value)
                {
                    layerID = value;
                    OnPropertyChanged();
                }
            }
        }

        private double scale = 1;
        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    OnPropertyChanged();
                }
            }
        }

        private double stackupProviderScale;
        public double StackupProviderScale
        {
            get { return stackupProviderScale; }
            set
            {
                if (stackupProviderScale != value)
                {
                    stackupProviderScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isStillLoaded;
        public bool IsStillLoaded
        {
            get { return isStillLoaded; }
            set
            {
                if(isStillLoaded != value)
                {
                    isStillLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isFrozen;
        public bool IsFrozen
        {
            get { return isFrozen; }
            set
            {
                if(isFrozen != value)
                {
                    isFrozen = value;
                    OnPropertyChanged();
                }
            }
        }

        private Thickness borderThickness;
        public Thickness BorderThickness
        {
            get { return borderThickness; }
            set
            {
                if (borderThickness != value)
                {
                    borderThickness = value;
                    OnPropertyChanged();
                }
            }
        }

        public int LayerX
        {
            get { return layerRect.X; }
        }

        public int LayerY
        {
            get { return layerRect.Y; }
        }

        public int LayerWidth
        {
            get { return layerRect.Width; }
        }

        public int LayerHeight
        {
            get { return layerRect.Height; }
        }

        private Rectangle pixelSpaceClippingRect;
        public Rectangle PixelSpaceClippingRect
        {
            get { return pixelSpaceClippingRect; }
            set
            {
                if (pixelSpaceClippingRect != value)
                {
                    pixelSpaceClippingRect = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle layerRect;
        public Rectangle LayerRect
        {
            get { return layerRect; }
            set
            {
                if (layerRect != value)
                {
                    layerRect = value;
                    OnPropertyChanged();
                    OnPropertyChanged("LayerX");
                    OnPropertyChanged("LayerY");
                    OnPropertyChanged("LayerWidth");
                    OnPropertyChanged("LayerHeight");
                }
            }
        }

        private Rectangle cloneRect;
        public Rectangle CloneRect
        {
            get { return cloneRect; }
            set
            {
                if (cloneRect != value)
                {
                    cloneRect = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle shadowRect;
        public Rectangle ShadowRect
        {
            get { return shadowRect; }
            set
            {
                if (shadowRect != value)
                {
                    shadowRect = value;
                    OnPropertyChanged();
                }
            }
        }

        private double shadowOpacity;
        public double ShadowOpacity
        {
            get { return shadowOpacity; }
            set
            {
                if(shadowOpacity != value)
                {
                    shadowOpacity = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle aoiRect;
        public Rectangle AOIRect
        {
            get { return aoiRect; }
            set
            {
                if (aoiRect != value)
                {
                    aoiRect = value;
                    OnPropertyChanged();
                }
            }
        }

        private Thickness aoiScaledMargin = new Thickness();
        public Thickness AOIScaledMargin
        {
            get { return aoiScaledMargin; }
            set
            {
                if(aoiScaledMargin != value)
                {
                    aoiScaledMargin = value;
                    OnPropertyChanged();
                }
            }
        }

        private Rectangle aoiScaledRect = new Rectangle();
        public Rectangle AOIScaledRect
        {
            get { return aoiScaledRect; }
            set
            {
                if (aoiScaledRect != value)
                {
                    aoiScaledRect = value;
                    OnPropertyChanged();
                }
            }
        }

        private KeyFrame keyFrame = new KeyFrame();
        public KeyFrame KeyFrame
        {
            get { return keyFrame; }
            set
            {
                if (keyFrame != value)
                {
                    keyFrame = value;
                    OnPropertyChanged();
                }
            }
        }

        private int pixelSpaceID;
        public int PixelSpaceID
        {
            get { return pixelSpaceID; }
            set
            {
                if (pixelSpaceID != value)
                {
                    pixelSpaceID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string thumbnail;
        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                if (thumbnail != value)
                {
                    thumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        private string windowLabel;
        public string WindowLabel
        {
            get { return windowLabel; }
            set
            {
                if(windowLabel != value)
                {
                    windowLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        private double opacity;
        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (opacity != value)
                {
                    bool wasVisible = opacity > 0;
                    bool isVisible = value > 0;

                    opacity = value;
                    OnPropertyChanged();

                    if (wasVisible != IsVisible)
                        OnPropertyChanged("IsVisible");
                }
            }
        }

        public bool IsVisible
        {
            get { return opacity > 0; }
        }

        private bool isHardwarePreview;
        public bool IsHardwarePreview
        {
            get { return isHardwarePreview; }
            set
            {
                if (isHardwarePreview != value)
                {
                    isHardwarePreview = value;
                    OnPropertyChanged();
                }
            }
        }

        private RenderObjectSource renderSource;
        public RenderObjectSource RenderSource
        {
            get { return renderSource; }
            set
            {
                if (renderSource != value)
                {
                    renderSource = value;
                    OnPropertyChanged();
                }
            }
        }


        public RenderLayer()
        {
        }

        public RenderLayer(RenderLayer copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public virtual void CopyFrom(RenderLayer copyFrom)
        {
            this.LayerID = copyFrom.layerID;
            this.PixelSpaceClippingRect = copyFrom.PixelSpaceClippingRect;
            this.LayerRect = copyFrom.LayerRect;
            this.CloneRect = copyFrom.CloneRect;
            this.ShadowRect = copyFrom.ShadowRect;
            this.ShadowOpacity = copyFrom.ShadowOpacity;
            this.AOIRect = copyFrom.AOIRect;
            this.AOIScaledMargin = copyFrom.AOIScaledMargin;
            this.AOIScaledRect = copyFrom.AOIScaledRect;
            this.KeyFrame.CopyFrom(copyFrom.KeyFrame);
            this.Opacity = copyFrom.opacity;
            this.Thumbnail = copyFrom.Thumbnail;
            this.Scale = copyFrom.Scale;
            this.PixelSpaceID = copyFrom.PixelSpaceID;
            this.RenderSource = copyFrom.RenderSource;
            this.IsHardwarePreview = copyFrom.IsHardwarePreview;
            this.ZIndex = copyFrom.ZIndex;
            this.BorderThickness = copyFrom.BorderThickness;
            this.StackupProviderScale = copyFrom.StackupProviderScale;
            this.WindowLabel = copyFrom.WindowLabel;
            this.IsStillLoaded = copyFrom.IsStillLoaded;
            this.IsFrozen = copyFrom.IsFrozen;
        }

        public void CopyFrom(DrawingData data, int layerIndex, MixerBus bus, IStackupProvider stackupProvider = null)
        {
            DrawingKeyFrame dkf = data.GetLayer(layerIndex, bus);
            if (dkf != null)
                CopyFrom(dkf, data.GetPixelSpace(dkf.PixelSpaceID), stackupProvider);
        }

        public void CopyFrom(DrawingKeyFrame dkf, DrawingPixelSpace parentPixelSpace, IStackupProvider stackupProvider = null)
        {
            if(parentPixelSpace == null)
            {
                this.Opacity = 0;
                return;
            }

            this.LayerRect = (stackupProvider == null ? dkf.LayerRect : stackupProvider.GenerateOffsetRect(dkf.LayerRect, dkf.PixelSpaceID));
            this.CloneRect = (stackupProvider == null ? dkf.CloneRect : stackupProvider.GenerateOffsetRect(dkf.CloneRect, dkf.PixelSpaceID));
            this.StackupProviderScale = (double)this.LayerRect.Width / ((double)dkf.LayerRect.Width / parentPixelSpace.Scale);
            this.AOIRect = dkf.AOIRect;
            this.Opacity = ((255 - dkf.Transparency) / (double)255);
            this.KeyFrame.CopyFrom(dkf.KeyFrame);
            this.WindowLabel = dkf.WindowLabel;
            this.Thumbnail = dkf.Thumbnail;
            this.Scale = dkf.Scale;
            this.PixelSpaceID = dkf.PixelSpaceID;
            this.IsHardwarePreview = dkf.IsHardwarePreview;
            this.RenderSource = RenderObjectSource.DrawingData;
            this.ZIndex = 10000 + dkf.LayerID;
            this.IsStillLoaded = !string.IsNullOrEmpty(dkf.LoadedStill);
            this.IsFrozen = dkf.IsFrozen;

            //Shadow Opacity is tricky.  Actual range is 128-255, but it's applied across a 0-255 scale.  Also is mixed with layer transparency
            this.ShadowOpacity = ((255 - dkf.Transparency) / (double)255) * ((256 - (128 + (dkf.KeyFrame.ShadowTransparency / 2))) / (double)256);

            //Shadow Rectangle is an offset from the base layer rect
            this.ShadowRect = new Rectangle()
            {
                X = (int)Math.Round(keyFrame.ShadowHOffset * StackupProviderScale),
                Y = (int)Math.Round(keyFrame.ShadowVOffset * StackupProviderScale),
                Width = this.LayerRect.Width + (int)Math.Round(keyFrame.ShadowHSize * StackupProviderScale),
                Height = this.LayerRect.Height + (int)Math.Round(keyFrame.ShadowHSize * StackupProviderScale)  //HACK:  ShadowVSize doesn't appear to be updated correctly - seems pinned to HSizing
            };

            //Clip recangle is the pixelspace rect offset for the layer position
            var clip = (stackupProvider == null ? parentPixelSpace.Rect : stackupProvider.GenerateOffsetRect(parentPixelSpace.Rect, parentPixelSpace.ID));
            clip.X -= (layerRect.X);
            clip.Y -= (layerRect.Y);
            this.PixelSpaceClippingRect = clip;


            //Scale the border thickness on our keyFrame to offset for rendered scaling
            this.BorderThickness = CalculateRenderBorderThickness(dkf.KeyFrame.BorderThickness, scale, dkf.LayerRect, layerRect, dkf.HActive, dkf.VActive);

            //Scaled AOI rectangles are used directly by UI components for rendering thumbnail cropping
            Thickness scaledAOIMargin;
            Rectangle scaledAOIRect;
            CalculateScaledAOI(dkf.HActive, dkf.VActive, this.LayerRect, this.AOIRect, this.BorderThickness, out scaledAOIMargin, out scaledAOIRect);
            this.AOIScaledMargin = scaledAOIMargin;
            this.AOIScaledRect = scaledAOIRect;
        }

        /// <summary>
        /// Builds a collection of render layers which represent a specific cue of a provided script element
        /// </summary>
        public async static Task<List<RenderLayer>> CreateFrom(DrawingData drawingData, ScriptElement element, int elementCueIndex, IRenderSceneDataProvider dataProvider, IStackupProvider stackupProvider = null)
        {
            if (element == null)
                return null;

            List<RenderLayer> response = new List<RenderLayer>();

            for (int i = 0; i < element.LayerCount; i++)
            {
                //If we are at the end of this element, remove all associated layers from screen
                if (elementCueIndex >= element.CueCount)
                {
                    response.Add(new RenderLayer() 
                    {
                        LayerID = element.StartLayer + i, 
                        Opacity = 0 
                    });
                    continue;
                }

                var layerProperties = GetRenderLayerProperties(drawingData, element, i, elementCueIndex);
                
                RenderLayer layer = new RenderLayer()
                {
                    LayerID = element.StartLayer + i,
                    Opacity = layerProperties.Item1,
                    PixelSpaceID = element.PixelSpaceID
                };
                layer.keyFrame.CopyFrom(layerProperties.Item2);

                response.Add(layer);

                var pixelSpace = await dataProvider.GetPixelSpace(element.PixelSpaceID);
                if (layer.Opacity == 0 || layerProperties.Item2 == null || pixelSpace == null)
                {
                    layer.Opacity = 0;
                    continue;
                }

                //Find / Create an input config needed for layer rect generation
                InputConfig ic = new InputConfig() { AspectRatio = 1.777f, HActive = 1920, VActive = 1080 };
                if (element is StillElement)
                {
                    //Generate layer properties for a still
                    layer.Thumbnail = layerProperties.Item3;

                    if (dataProvider != null)
                    {
                        //Lookup image size

                        Size dimensions = await dataProvider.GetImageFileDimensions(layer.thumbnail);
                        if (dimensions.Width != 0 && dimensions.Height != 0)
                        {
                            ic.HActive = dimensions.Width;
                            ic.VActive = dimensions.Height;
                            ic.AspectRatio = (float)dimensions.Width / (float)dimensions.Height;
                        }
                    }
                }
                else
                {
                    //Generate layer properties for a source
                    string sourceName = layerProperties.Item3;
                    if (!string.IsNullOrEmpty(sourceName))
                    {
                        Source source = await dataProvider.GetSource(sourceName);
                        if (source != null)
                        {
                            layer.Thumbnail = source.Thumbnail;

                            if (source.InputConfigID != -1)
                            {
                                var newIc = await dataProvider.GetInputConfig(source.InputConfigID);
                                if (newIc != null)
                                    ic = newIc;
                            }
                        }
                    }
                }

                var layerRect = LayerHelpers.GetAbsoluteRectangle(layer.KeyFrame, ic.AspectRatio, pixelSpace);
                var cloneRect = LayerHelpers.GetCloneRectangle(layer.KeyFrame, pixelSpace.Rect, layerRect);

                layer.LayerRect = (stackupProvider == null ? layerRect : stackupProvider.GenerateOffsetRect(layerRect, pixelSpace.ID));
                layer.CloneRect = (stackupProvider == null ? layerRect : stackupProvider.GenerateOffsetRect(cloneRect, pixelSpace.ID));
                layer.StackupProviderScale = (double)layer.LayerRect.Width / ((double)layerRect.Width / pixelSpace.Scale);
                layer.AOIRect = LayerHelpers.GetAOIRectangle(layer.KeyFrame, ic);
                layer.Scale = pixelSpace.Scale;
                layer.PixelSpaceID = pixelSpace.ID;
                layer.Opacity = 1;
                layer.IsHardwarePreview = false;
                layer.RenderSource = RenderObjectSource.Script;
                layer.ZIndex = 10000 + element.StartLayer;

                //Clip recangle is the pixelspace rect offset for the layer position
                var clip = (stackupProvider == null ? pixelSpace.Rect : stackupProvider.GenerateOffsetRect(pixelSpace.Rect, pixelSpace.ID));
                clip.X -= (layer.LayerRect.X);
                clip.Y -= (layer.LayerRect.Y);
                layer.PixelSpaceClippingRect = clip;

                //Scale the border thickness on our keyFrame to offset for rendered scaling
                layer.BorderThickness = CalculateRenderBorderThickness(
                    layer.KeyFrame.BorderThickness,
                    pixelSpace.Scale,
                    layerRect,
                    layer.LayerRect,
                    (ic == null ? 1920 : ic.HActive),
                    (ic == null ? 1080 : ic.VActive));

                //Scaled AOI rectangles are used directly by UI components for rendering thumbnail cropping
                Thickness scaledAOIMargin;
                Rectangle scaledAOIRect;
                CalculateScaledAOI(ic.HActive, ic.VActive, layer.LayerRect, layer.AOIRect, layer.BorderThickness, out scaledAOIMargin, out scaledAOIRect);
                layer.AOIScaledMargin = scaledAOIMargin;
                layer.AOIScaledRect = scaledAOIRect;
            }

            return response;
        }

        private static Tuple<double, KeyFrame, string> GetRenderLayerProperties(DrawingData currentDrawingData, ScriptElement element, int layerOffset, int elementCueOffset)
        {
            string sourceName = string.Empty;
            double opacity = 0;
            KeyFrame keyFrame = null;

            if (element is OffElement || elementCueOffset == 0 || elementCueOffset >= (element.StartCue + element.CueCount))
            {
                opacity = 0;
            }
            else if (element is SourceElement)
            {
                if (layerOffset == 0)
                {
                    sourceName = ((SourceElement)element).SourceName;
                    opacity = 1;
                    keyFrame = element.GetDrivingKeyFrame(elementCueOffset, ElementIndexRelativeTo.Element);
                }
            }
            else if (element is StillElement)
            {
                if (layerOffset == 0)
                {
                    sourceName = ((StillElement)element).FileName;
                    opacity = 1;
                    keyFrame = element.GetDrivingKeyFrame(elementCueOffset, ElementIndexRelativeTo.Element);
                }
            }
            else if (element is MixerElement)
            {
                string drivingSource = element.GetDrivingSource(elementCueOffset, ElementIndexRelativeTo.Element);
                KeyFrame drivingKf = element.GetDrivingKeyFrame(elementCueOffset, ElementIndexRelativeTo.Element);

                if (!string.IsNullOrEmpty(drivingSource) && drivingKf != null)
                {
                    int pgmLayerID = element.StartLayer;

                    //Determine the program side of the mixer currently
                    if (currentDrawingData != null)
                    {
                        //Determine current layer state, and if a mixer transition is required
                        var layerA = currentDrawingData.GetLayer(element.StartLayer, MixerBus.Program);
                        var layerB = currentDrawingData.GetLayer(element.StartLayer + 1, MixerBus.Program);
                        if (layerA != null && layerB != null)
                        {
                            DrawingKeyFrame pgmLayer, pvwLayer;
                            GetMixerTopAndBottomLayers(layerA, layerB, element.PixelSpaceID, drivingSource, drivingKf, out pgmLayer, out pvwLayer);
                            pgmLayerID = pgmLayer.LayerID;

                            //If PGM does not match the target KF and source, then a mixer transition will occur
                            if (drivingKf.Equals(pgmLayer.KeyFrame) && string.Compare(drivingSource, pgmLayer.Source, StringComparison.CurrentCultureIgnoreCase) == 0)
                                pgmLayerID = pgmLayer.LayerID;
                            else
                                pgmLayerID = pvwLayer.LayerID;
                        }
                    }

                    //Show layer contents only if my layer is the PGM layer
                    int myLayerID = element.StartLayer + layerOffset;
                    if (myLayerID == pgmLayerID)
                    {
                        opacity = 1;
                        keyFrame = new KeyFrame(drivingKf);
                        sourceName = drivingSource;
                    }
                    else
                    {
                        //TODO: Show this layer in a preview pixelspace (if one exists)
                    }
                }
            }
            else if (element is InputArrayElement)
            {
                if (element.KeyFrames.ContainsKey(layerOffset) && element.SourceNames.ContainsKey(layerOffset))
                {
                    opacity = 1;
                    keyFrame = new KeyFrame(element.KeyFrames[layerOffset]);
                    sourceName = element.SourceNames[layerOffset];
                }
            }

            return new Tuple<double, KeyFrame, string>(opacity, keyFrame, sourceName);
        }

        protected static void GetMixerTopAndBottomLayers(DrawingKeyFrame layerA, DrawingKeyFrame layerB, int targetPixelSpaceID, string targetSource, KeyFrame targetKeyFrame, out DrawingKeyFrame pgmLayer, out DrawingKeyFrame pvwLayer)
        {
            //Run through a number of possible scenarios, following system logic, to determine layer pgm/pvw states
            if (layerA.PixelSpaceID == targetPixelSpaceID && layerA.IsVisible && layerA.Source == targetSource && layerA.KeyFrame.Equals(targetKeyFrame))
            {
                //Layer A is already matches the target pixelspace, keyframe, and source
                pgmLayer = layerA;
                pvwLayer = layerB;
            }
            else if (layerA.PixelSpaceID == targetPixelSpaceID && layerA.IsVisible && layerB.Source == targetSource && layerB.KeyFrame.Equals(targetKeyFrame))
            {
                //Layer B is already matches the target pixelspace, keyframe, and source
                pgmLayer = layerB;
                pvwLayer = layerA;
            }
            else if (layerA.PixelSpaceID == targetPixelSpaceID && layerA.IsVisible)
            {
                //Layer A is already visible in the target pixelspace
                pgmLayer = layerA;
                pvwLayer = layerB;
            }
            else if (layerB.PixelSpaceID == targetPixelSpaceID && layerB.IsVisible)
            {
                //Layer B is already in the target pixelspace
                pgmLayer = layerB;
                pvwLayer = layerA;
            }
            else if (layerA.IsVisible && layerA.Scale == 1)
            {
                //Layer A is already in a program PixelSpace
                pgmLayer = layerA;
                pvwLayer = layerB;
            }
            else if (layerB.IsVisible && layerB.Scale == 1)
            {
                //Layer B is already in a program PixelSpace
                pgmLayer = layerB;
                pvwLayer = layerA;
            }
            else
            {
                //Neither layer is in a PGM state, so the first layer will become PGM
                pgmLayer = layerA;
                pvwLayer = layerB;
            }
        }

        private static Thickness CalculateRenderBorderThickness(int borderThickness, double pixelSpaceScale, Rectangle layerNativeRect, Rectangle layerRenderRect, int layerHActive, int layerVActive)
        {
            Thickness effectiveThickness = new Thickness(borderThickness);
            if (borderThickness > 0)
            {
                //Sanity checks
                if (layerHActive <= 0)
                    layerHActive = 1024;
                if (layerVActive <= 0)
                    layerVActive = 768;

                //Actual border thickness is based on the native resolution of the input
                double layerHScale = Math.Min(1, (double)layerHActive / (double)layerNativeRect.Width);
                double layerVScale = Math.Min(1, (double)layerVActive / (double)layerNativeRect.Height);
                effectiveThickness.Left = (borderThickness * layerHScale);
                effectiveThickness.Top = (borderThickness * layerVScale);

                //Offset by pixelspace scale
                if (pixelSpaceScale != 1)
                {
                    effectiveThickness.Left *= pixelSpaceScale;
                    effectiveThickness.Top *= pixelSpaceScale;
                }

                //Offset by render scaling
                if (layerNativeRect.Width != layerRenderRect.Width && layerRenderRect.Width > 0)
                {
                    double renderScale = (double)layerRenderRect.Width / (double)layerNativeRect.Width;
                    effectiveThickness.Left *= renderScale;
                    effectiveThickness.Top *= renderScale;
                }

                //Round up to prevent visual rounding issues, and copy right/bottom from left/top
                effectiveThickness.Left = effectiveThickness.Right = Math.Ceiling(effectiveThickness.Left);
                effectiveThickness.Top = effectiveThickness.Bottom = Math.Ceiling(effectiveThickness.Top);
            }

            return effectiveThickness;
        }
        
        private static void CalculateScaledAOI(int hActive, int vActive, Rectangle layerRect, Rectangle aoiRect, Thickness borderThickness, out Thickness aoiScaledMargin, out Rectangle aoiScaledRect)
        {
            if (aoiRect.Left == 0 && aoiRect.Top == 0 && aoiRect.Right == hActive && aoiRect.Bottom == vActive)
            {
                aoiScaledRect = new Rectangle(0, 0, layerRect.Width, layerRect.Height);
                aoiScaledMargin = new Thickness();
            }
            else
            {
                //Determine how much source crop is present, in percent
                double leftPercent = (aoiRect.Left <= 0 ? 0 : (double)aoiRect.Left / hActive);
                double topPercent = (aoiRect.Top <= 0 ? 0 : (double)aoiRect.Top / vActive);
                double rightPercent = (aoiRect.Right >= hActive ? 0 : (hActive - (double)aoiRect.Right) / hActive);
                double bottomPercent = (aoiRect.Bottom >= vActive ? 0 : (vActive - (double)aoiRect.Bottom) / vActive);

                //Determine the uncropped size of the layer
                double totalHCrop = leftPercent + rightPercent;
                double totalVCrop = topPercent + bottomPercent;

                double uncroppedWidth; 
                if (totalHCrop == 0)
                    uncroppedWidth = layerRect.Width;
                if (totalHCrop >= 1)
                    uncroppedWidth = 0;
                else
                    uncroppedWidth = layerRect.Width / (1 - totalHCrop);

                double uncroppedHeight;
                if (totalVCrop == 0)
                    uncroppedHeight = layerRect.Height;
                else if (totalVCrop >= 1)
                    uncroppedHeight = 0;
                else
                    uncroppedHeight = layerRect.Height / (1 - totalVCrop);

                aoiScaledMargin = new Thickness()
                {
                    Left = 0 - (uncroppedWidth * leftPercent),
                    Top = 0 - (uncroppedHeight * topPercent),
                    Right = 0 - (uncroppedWidth * rightPercent),
                    Bottom = 0 - (uncroppedHeight * bottomPercent)
                };

                aoiScaledRect = new Rectangle()
                {
                    X = (int)(uncroppedWidth * leftPercent),
                    Y = (int)(uncroppedHeight * topPercent),
                    Width = (int)Math.Max(0, Math.Ceiling(layerRect.Width - borderThickness.Left - borderThickness.Right)),
                    Height = (int)Math.Max(0, Math.Ceiling(layerRect.Height - borderThickness.Top - borderThickness.Bottom))
                };
            }
        }
    }
}
