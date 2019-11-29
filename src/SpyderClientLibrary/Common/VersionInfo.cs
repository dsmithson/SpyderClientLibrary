namespace Spyder.Client.Common
{
    public class VersionInfo : PropertyChangedBase
    {
        private int major;
        public int Major
        {
            get { return major; }
            set
            {
                if (major != value)
                {
                    major = value;
                    OnPropertyChanged();
                }
            }
        }

        private int minor;
        public int Minor
        {
            get { return minor; }
            set
            {
                if (minor != value)
                {
                    minor = value;
                    OnPropertyChanged();
                }
            }
        }

        private int build;
        public int Build
        {
            get { return build; }
            set
            {
                if (build != value)
                {
                    build = value;
                    OnPropertyChanged();
                }
            }
        }

        public VersionInfo()
        {
        }

        public VersionInfo(int major, int minor, int build)
        {
            this.major = major;
            this.minor = minor;
            this.build = build;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as VersionInfo;
            if (compareTo == null)
                return false;

            if (compareTo.major == this.major && compareTo.minor == this.minor && compareTo.build == this.build)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public string ToShortString()
        {
            return string.Format("{0}.{1}.{2}", major, minor, build);
        }

        public override string ToString()
        {
            return string.Format("Version {0}.{1}.{2}", major, minor, build);
        }
    }
}
