using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class TimeCode : PropertyChangedBase
    {
        private int hours;
        public int Hours
        {
            get { return hours; }
            set
            {
                if (hours != value)
                {
                    hours = value;
                    OnPropertyChanged();
                }
            }
        }

        private int minutes;
        public int Minutes
        {
            get { return minutes; }
            set
            {
                if (minutes != value)
                {
                    minutes = value;
                    OnPropertyChanged();
                }
            }
        }

        private int seconds;
        public int Seconds
        {
            get { return seconds; }
            set
            {
                if (seconds != value)
                {
                    seconds = value;
                    OnPropertyChanged();
                }
            }
        }

        private int frames;
        public int Frames
        {
            get { return frames; }
            set
            {
                if (frames != value)
                {
                    frames = value;
                    OnPropertyChanged();
                }
            }
        }

        private FieldRate fieldRate;
        public FieldRate FieldRate
        {
            get { return fieldRate; }
            set
            {
                if (fieldRate != value)
                {
                    fieldRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public int FramesPerSecond()
        {
            return FramesPerSecond(fieldRate);
        }

        public static int FramesPerSecond(FieldRate fieldRate)
        {
            if (fieldRate == FieldRate.FR_60)
            {
                return 60;
            }
            else if (fieldRate == FieldRate.NTSC || fieldRate == FieldRate.FR_30 || fieldRate == FieldRate.FR_29_97)
            {
                return 30;
            }
            else if (fieldRate == FieldRate.FR_48)
            {
                return 48;
            }
            else if (fieldRate == FieldRate.PAL || fieldRate == FieldRate.FR_25)
            {
                return 25;
            }
            else // if (fps == FieldRate.FR_23_98 || fps == FieldRate.FR_24)
            {
                return 24;
            }
        }

        public override string ToString()
        {
            string s = string.Format("{0:d2}:{1:d2}:{2:d2}.{3:d2}", hours, minutes, seconds, frames);
            return s;
        }
        public void Set(TimeCode t)
        {
            this.hours = t.Hours;
            this.minutes = t.Minutes;
            this.seconds = t.Seconds;
            this.frames = t.Frames;
        }
        public void Set(long frames)
        {
            long total = frames;
            long f = (long)FramesPerSecond();

            Hours = (int)(total / 60L / 60L / f);
            total -= (long)(hours * 60 * 60 * f);

            Minutes = (int)(total / 60L / f);
            total -= (long)(minutes * 60 * f);

            Seconds = (int)(total / f);
            total -= (long)(seconds * f);

            Frames = (int)(frames % f);
            total -= frames;
        }
        public void Set(FieldRate rate, long frames)
        {
            this.FieldRate = rate;
            Set(frames);
        }
    }
}
