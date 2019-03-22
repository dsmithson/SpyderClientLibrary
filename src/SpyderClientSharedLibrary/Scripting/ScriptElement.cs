using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Net.DrawingData;
using Knightware.Primitives;

namespace Spyder.Client.Scripting
{
    public enum ElementIndexRelativeTo { ParentScript, Element };

    public class ScriptElement : PropertyChangedBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
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

        private int startLayer;
        public int StartLayer
        {
            get { return startLayer; }
            set
            {
                if (startLayer != value)
                {
                    startLayer = value;
                    OnPropertyChanged();
                }
            }
        }

        private int layerCount;
        public int LayerCount
        {
            get { return layerCount; }
            set
            {
                if (layerCount != value)
                {
                    layerCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int startCue;
        public int StartCue
        {
            get { return startCue; }
            set
            {
                if (startCue != value)
                {
                    startCue = value;
                    OnPropertyChanged();
                }
            }
        }

        private int cueCount;
        public int CueCount
        {
            get { return cueCount; }
            set
            {
                if (cueCount != value)
                {
                    cueCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int mixOnRate;
        public int MixOnRate
        {
            get { return mixOnRate; }
            set
            {
                if (mixOnRate != value)
                {
                    mixOnRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private int mixOffRate;
        public int MixOffRate
        {
            get { return mixOffRate; }
            set
            {
                if (mixOffRate != value)
                {
                    mixOffRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private int entranceEffectID;
        public int EntranceEffectID
        {
            get { return entranceEffectID; }
            set
            {
                if (entranceEffectID != value)
                {
                    entranceEffectID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int exitEffectID;
        public int ExitEffectID
        {
            get { return exitEffectID; }
            set
            {
                if (exitEffectID != value)
                {
                    exitEffectID = value;
                    OnPropertyChanged();
                }
            }
        }

        private SlideTransitionType entranceEffectType;
        public SlideTransitionType EntranceEffectType
        {
            get { return entranceEffectType; }
            set
            {
                if (entranceEffectType != value)
                {
                    entranceEffectType = value;
                    OnPropertyChanged();
                }
            }
        }

        private SlideTransitionType exitEffectType;
        public SlideTransitionType ExitEffectType
        {
            get { return exitEffectType; }
            set
            {
                if (exitEffectType != value)
                {
                    exitEffectType = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isDisabled;
        public bool IsDisabled
        {
            get { return isDisabled; }
            set
            {
                if (isDisabled != value)
                {
                    isDisabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, KeyFrame> keyFrames = new Dictionary<int,KeyFrame>();
        public Dictionary<int, KeyFrame> KeyFrames
        {
            get { return keyFrames; }
            set
            {
                if (keyFrames != value)
                {
                    keyFrames = value;
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<int, Content> contents = new Dictionary<int, Content>();
        public Dictionary<int, Content> Contents
        {
            get { return contents; }
            set
            {
                if(contents != value)
                {
                    contents = value;
                    OnPropertyChanged();
                }
            }
        }
        
        [Obsolete("Please migrate code consuming this property to use the Contents property instead")]
        public Dictionary<int, string> SourceNames
        {
            get { return contents.ToDictionary(c => c.Key, c => c.Value.Name); }
        }

        public virtual KeyFrame GetDrivingKeyFrame(int cueIndex, ElementIndexRelativeTo relativeTo)
        {
            if (relativeTo == ElementIndexRelativeTo.ParentScript)
                cueIndex -= (this.startCue - 1);

            while (cueIndex >= 0)
            {
                if (keyFrames.ContainsKey(cueIndex))
                    return keyFrames[cueIndex];

                cueIndex--;
            }
            return null;
        }

        public virtual Content GetDrivingContent(int cueIndex, ElementIndexRelativeTo relativeTo)
        {
            if (relativeTo == ElementIndexRelativeTo.ParentScript)
                cueIndex -= (this.startCue - 1);

            while (cueIndex >= 0)
            {
                if (contents.ContainsKey(cueIndex))
                    return contents[cueIndex];

                cueIndex--;
            }
            return null;
        }

        [Obsolete("Please migrate code consuming this property to use the GetDrivingContent method instead")]
        public virtual string GetDrivingSource(int cueIndex, ElementIndexRelativeTo relativeTo)
        {
            return GetDrivingContent(cueIndex, relativeTo)?.Name;
        }
    }
}
