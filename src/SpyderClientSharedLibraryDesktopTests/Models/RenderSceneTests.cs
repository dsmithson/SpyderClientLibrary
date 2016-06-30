using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spyder.Client.Common;
using Spyder.Client.Net.DrawingData;
using Knightware.Primitives;
using Spyder.Client.Scripting;

namespace Spyder.Client.Models
{
    [TestClass]
    public class RenderSceneTests
    {
        /// <summary>
        /// Tests to ensure an off element provided on a script cue removes the associated layer from drawing data
        /// </summary>
        [TestMethod]
        public async Task RelativeScriptOffElementTest()
        {
            var drawingData = BuildDrawingData(4, visibleLayerIDs: new int[] { 2, 3 });
            var script = BuildScript(true, new OffElement()
            {
                StartCue = 1,
                CueCount = 1,
                StartLayer = 2,
                LayerCount = 1
            });

            //Build a render scene
            var renderScene = await BuildScene(drawingData, script, 1);

            //Assert that layer 2 is not visible
            var layer2 = renderScene.AllRenderSceneObjects.OfType<RenderLayer>().FirstOrDefault(l => l.LayerID == 2);
            Assert.IsTrue(layer2 == null || !layer2.IsVisible, "Layer 2 was not removed from the render scene even though there was an associated off element");

            //Assert that layer 3 is visible
            var layer3 = renderScene.AllRenderSceneObjects.OfType<RenderLayer>().FirstOrDefault(l => l.LayerID == 3);
            Assert.IsTrue(layer3 != null && layer3.IsVisible, "Layer 3 should have been visible in the render scene");
        }

        [TestMethod]
        public async Task RelativeScriptMixerElementSourceTest()
        {
            await RelativeScriptMixerElementTest(mixerElement =>
                {
                    //Create a different source at cue 2
                    mixerElement.SourceNames.Add(2, mixerElement.SourceNames[0] + " - Copy");
                });
        }

        [TestMethod]
        public async Task RelativeScriptMixerElementKeyFrameTest()
        {
            await RelativeScriptMixerElementTest(mixerElement =>
            {
                //Create a different keyframe at cue 2
                var kf = new KeyFrame(mixerElement.KeyFrames[0]);
                kf.HPosition += 0.25f;

                mixerElement.KeyFrames.Add(2, kf);
            });
        }

        private async Task RelativeScriptMixerElementTest(Action<MixerElement> makeChangesToCueIndex2)
        {
            var drawingData = BuildDrawingData(6, visibleLayerIDs: new int[] { 2 });
            var onscreenLayer = drawingData.DrawingKeyFrames[2];
            var mixerElement = new MixerElement()
            {
                StartLayer = 2,
                LayerCount = 2,
                StartCue = 1,
                CueCount = 3,
                PixelSpaceID = onscreenLayer.PixelSpaceID
            };
            var script = BuildScript(true, mixerElement);

            //Mirror the source and keyframe for the onscreen layer.  This should cause nothing to happen when we
            //recall cue 1, since everything matches
            mixerElement.SourceNames.Add(0, onscreenLayer.Source);
            mixerElement.KeyFrames.Add(0, new KeyFrame(onscreenLayer.KeyFrame));

            //Make some change(s) to cue index 2 that should cause a mixer transition
            makeChangesToCueIndex2(mixerElement);

            //Now lets run cue 1 and ensure layer 1 is still on screen
            var scene = await BuildScene(drawingData, script, 1);
            var sceneLayer2 = scene.AllRenderSceneObjects.OfType<RenderLayer>().FirstOrDefault(l => l.LayerID == 2);
            var sceneLayer3 = scene.AllRenderSceneObjects.OfType<RenderLayer>().FirstOrDefault(l => l.LayerID == 3);
            Assert.IsNotNull(sceneLayer2, "After cue 1 scene generation, layer 2 was not included in the scene");
            Assert.IsTrue(sceneLayer2.IsVisible, "After cue 1 scene generation, layer 2 was not visible");
            UnitTestHelper.MemberwiseCompareAssert(onscreenLayer.KeyFrame, sceneLayer2.KeyFrame, "After cue 1 scene generation, layer 2 keyframe was incorrect");
            if (sceneLayer3 != null)
            {
                Assert.IsFalse(sceneLayer3.IsVisible, "After cue 1 scene generation, Layer 3 should not be visible");
            }

            //Now lets run cue 2 and ensure a mixer transition occurs, causing our new source and keyframe to be on layer 3
            scene = await BuildScene(drawingData, script, 2);
            sceneLayer2 = scene.AllRenderSceneObjects.OfType<RenderLayer>().FirstOrDefault(l => l.LayerID == 2);
            sceneLayer3 = scene.AllRenderSceneObjects.OfType<RenderLayer>().FirstOrDefault(l => l.LayerID == 3);
            Assert.IsNotNull(sceneLayer3, "After cue 2 scene generation, layer 3 was not included in the scene");
            Assert.IsTrue(sceneLayer3.IsVisible, "After cue 2 scene generation, layer 3 was not visible");
            UnitTestHelper.MemberwiseCompareAssert(mixerElement.GetDrivingKeyFrame(2, ElementIndexRelativeTo.ParentScript), sceneLayer3.KeyFrame, "After cue 2 scene generation, layer 3 keyframe was incorrect");
            if (sceneLayer2 != null)
            {
                Assert.IsFalse(sceneLayer2.IsVisible, "After cue 2 scene generation, Layer 2 should not be visible");
            }

            //Now lets run cue 3, which should cause the mixer to appear off screen for both layers
            scene = await BuildScene(drawingData, script, 3);
            sceneLayer2 = scene.AllRenderSceneObjects.OfType<RenderLayer>().FirstOrDefault(l => l.LayerID == 2);
            sceneLayer3 = scene.AllRenderSceneObjects.OfType<RenderLayer>().FirstOrDefault(l => l.LayerID == 3);
            if (sceneLayer2 != null)
            {
                Assert.IsFalse(sceneLayer2.IsVisible, "After cue 3 scene generation, Layer 2 should not be visible");
            }
            if (sceneLayer3 != null)
            {
                Assert.IsFalse(sceneLayer3.IsVisible, "After cue 3 scene generation, Layer 3 should not be visible");
            }
        }

        private async Task<RenderScene> BuildScene(DrawingData drawingData, Script script, int scriptCue)
        {
            var response = new RenderScene(new MockRenderSceneDataProvider());
            await response.Update(drawingData, script, scriptCue);
            return response;
        }

        private Script BuildScript(bool isRelative, params ScriptElement[] elements)
        {
            var response = new Script()
            {
                ID = 0,
                Name= "Test Script",
                IsRelative = isRelative
            };

            //Set cues
            int cues = Math.Min(3, elements.Max(e => e.StartCue + e.CueCount));
            for (int i = 0; i < cues; i++)
            {
                response.Cues.Add(new ScriptCue()
                {
                    ID = i,
                    Name = "Cue " + i
                });
            }

            //Set elements
            response.Elements.AddRange(elements);

            return response;
        }

        private Spyder.Client.Net.DrawingData.DrawingData BuildDrawingData(int layerCount, params int[] visibleLayerIDs)
        {
            var response = new Spyder.Client.Net.DrawingData.DrawingData();

            response.PixelSpaces.Add(0, new DrawingPixelSpace()
            {
                ID = 0,
                Name = "Test PixelSpace",
                Rect = new Rectangle(0, 0, 2560, 1024),
            });

            for (int i = 0; i < layerCount; i++)
            {
                response.DrawingKeyFrames.Add(i, new DrawingKeyFrame()
                {
                    LayerID = i,
                    IsBackground = (i < 2),
                    LayerRect = new Rectangle(0, 0, 1024, 768),
                    AOIRect = new Rectangle(0, 0, 1024, 768),
                    Source = "Source 1",
                    HActive = 1024,
                    VActive = 768,
                    PixelSpaceID = 0,
                    IsWithinPixelSpace = true,
                    Transparency = (byte)(visibleLayerIDs != null && visibleLayerIDs.Contains(i) ? 0 : 255)
                });
            }

            return response;
        }
    }
}
