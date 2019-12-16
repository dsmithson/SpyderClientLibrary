using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Scripting
{
    public class ScriptCue : PropertyChangedBase
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

        private JumpType jumpType;
        public JumpType JumpType
        {
            get { return jumpType; }
            set
            {
                if (jumpType != value)
                {
                    jumpType = value;
                    OnPropertyChanged();
                }
            }
        }

        private int jumpTo;
        public int JumpTo
        {
            get { return jumpTo; }
            set
            {
                if (jumpTo != value)
                {
                    jumpTo = value;
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

        private TimeCode triggerTime;
        public TimeCode TriggerTime
        {
            get { return triggerTime; }
            set
            {
                if (triggerTime != value)
                {
                    triggerTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private CueTrigger triggerType;
        public CueTrigger TriggerType
        {
            get { return triggerType; }
            set
            {
                if (triggerType != value)
                {
                    triggerType = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<KeyValuePair<int, PlayItemCommand>> playItems;
        public List<KeyValuePair<int, PlayItemCommand>> PlayItems
        {
            get { return playItems; }
            set
            {
                if (playItems != value)
                {
                    playItems = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<int> functionKeyIDs;
        public List<int> FunctionKeyIDs
        {
            get { return functionKeyIDs; }
            set
            {
                if (functionKeyIDs != value)
                {
                    functionKeyIDs = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
