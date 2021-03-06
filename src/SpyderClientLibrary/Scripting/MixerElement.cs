﻿namespace Spyder.Client.Scripting
{
    public class MixerElement : ScriptElement
    {
        private bool thruBlack;
        public bool ThruBlack
        {
            get { return thruBlack; }
            set
            {
                if (thruBlack != value)
                {
                    thruBlack = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool morph;
        public bool Morph
        {
            get { return morph; }
            set
            {
                if (morph != value)
                {
                    morph = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
