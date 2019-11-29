using System.Collections.Generic;
using System.Linq;

namespace Spyder.Client.Scripting
{
    public class Script : PropertyChangedBase
    {
        private int id;
        public int ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private int revision;
        public int Revision
        {
            get { return revision; }
            set
            {
                if (revision != value)
                {
                    revision = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isRelative;
        public bool IsRelative
        {
            get { return isRelative; }
            set
            {
                if (isRelative != value)
                {
                    isRelative = value;
                    OnPropertyChanged();
                }
            }
        }

        private int clockSourceID;
        public int ClockSourceID
        {
            get { return clockSourceID; }
            set
            {
                if (clockSourceID != value)
                {
                    clockSourceID = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<ScriptElement> elements = new List<ScriptElement>();
        public List<ScriptElement> Elements
        {
            get { return elements; }
            set
            {
                if (elements != value)
                {
                    elements = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<ScriptCue> cues = new List<ScriptCue>();
        public List<ScriptCue> Cues
        {
            get { return cues; }
            set
            {
                if (cues != value)
                {
                    cues = value;
                    OnPropertyChanged();
                }
            }
        }

        public ScriptElement GetElement(int layerID, int cueIndex)
        {
            foreach (var element in elements)
            {
                if (element.StartLayer <= layerID && (element.StartLayer + element.LayerCount) > layerID)
                {
                    if (element.StartCue <= cueIndex && (element.StartCue + element.CueCount) > cueIndex)
                    {
                        return element;
                    }
                }
            }

            //Not found
            return null;
        }

        public ScriptCue GetCue(int cueIndex)
        {
            if (cues == null || cueIndex < 0 || cueIndex >= cues.Count)
                return null;

            return cues.FirstOrDefault(cue => cue.ID == cueIndex);
        }

        public IEnumerable<ScriptElement> GetElements(int cueIndex)
        {
            List<ScriptElement> response = new List<ScriptElement>();
            foreach (var element in elements)
            {
                //if(cueIndex >= (element.StartCue - 1) && cueIndex <= (element.StartCue + element.CueCount))
                if (element.StartCue + element.CueCount > cueIndex || element.StartCue < cueIndex)
                {
                    response.Add(element);
                }
            }
            return response;
        }
    }
}
