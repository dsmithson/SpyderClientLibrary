using System;

namespace Spyder.Client.Common
{
    public class TimeCode : IEquatable<TimeCode>
    {
        private readonly int hours;
        public int Hours
        {
            get { return hours; }
        }

        private readonly int minutes;
        public int Minutes
        {
            get { return minutes; }
        }

        private readonly int seconds;
        public int Seconds
        {
            get { return seconds; }
        }

        private readonly int frames;
        public int Frames
        {
            get { return frames; }
        }

        private readonly FieldRate fieldRate;
        public FieldRate FieldRate
        {
            get { return fieldRate; }
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

        public TimeCode(FieldRate fieldRate = FieldRate.NTSC, int hours = 0, int minutes = 0, int seconds = 0, int frames = 0)
        {
            this.fieldRate = fieldRate;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.frames = frames;
        }

        public TimeCode(FieldRate fieldRate, long frames)
        {
            this.fieldRate = fieldRate;

            long total = frames;
            long f = FramesPerSecond();

            this.hours = (int)(total / 60L / 60L / f);
            total -= (hours * 60 * 60 * f);

            this.minutes = (int)(total / 60L / f);
            total -= (minutes * 60 * f);

            this.seconds = (int)(total / f);

            this.frames = (int)(frames % f);
        }

        public bool Equals(TimeCode other)
        {
            if (other == null)
                return false;
            else if (this.hours != other.hours)
                return false;
            else if (this.minutes != other.minutes)
                return false;
            else if (this.seconds != other.seconds)
                return false;
            else if (frames != other.frames)
                return false;
            else if (this.fieldRate != other.fieldRate)
                return false;
            else
                return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TimeCode);
        }

        public override int GetHashCode()
        {
            return (((((int)fieldRate * 251) + hours) * 251 + minutes) * 251 + seconds) * 251 + frames;
        }

        public static bool operator ==(TimeCode tc1, TimeCode tc2)
        {
            if (tc1 is null || tc2 is null)
                return Object.Equals(tc1, tc2);

            return tc1.Equals(tc2);
        }

        public static bool operator !=(TimeCode tc1, TimeCode tc2)
        {
            if (tc1 is null || tc2 is null)
                return !Object.Equals(tc1, tc2);

            return !tc1.Equals(tc2);
        }
    }
}
