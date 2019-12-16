using System;

namespace Spyder.Client.Common
{
    public class Source : Register, IEquatable<Source>
    {
        private int routerID;
        public int RouterID
        {
            get { return routerID; }
            set
            {
                if (routerID != value)
                {
                    routerID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int routerInput;
        public int RouterInput
        {
            get { return routerInput; }
            set
            {
                if (routerInput != value)
                {
                    routerInput = value;
                    OnPropertyChanged();
                }
            }
        }

        private int inputConfigID;
        public int InputConfigID
        {
            get { return inputConfigID; }
            set
            {
                if (inputConfigID != value)
                {
                    inputConfigID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int preferredTreatmentID;
        public int PreferredTreatmentID
        {
            get { return preferredTreatmentID; }
            set
            {
                if (preferredTreatmentID != value)
                {
                    preferredTreatmentID = value;
                    OnPropertyChanged();
                }
            }
        }

        private int preferredLayerID;
        public int PreferredLayerID
        {
            get { return preferredLayerID; }
            set
            {
                if (preferredLayerID != value)
                {
                    preferredLayerID = value;
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

        private string linearKeySource;
        public string LinearKeySource
        {
            get { return linearKeySource; }
            set
            {
                if (linearKeySource != value)
                {
                    linearKeySource = value;
                    OnPropertyChanged();
                }
            }
        }

        public Source()
        {
        }

        public Source(IRegister copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public override void CopyFrom(IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            Source myCopyFrom = copyFrom as Source;
            if (myCopyFrom != null)
            {
                this.RouterID = myCopyFrom.RouterID;
                this.RouterInput = myCopyFrom.RouterInput;
                this.InputConfigID = myCopyFrom.InputConfigID;
                this.PreferredTreatmentID = myCopyFrom.PreferredTreatmentID;
                this.PreferredLayerID = myCopyFrom.PreferredLayerID;
                this.Thumbnail = myCopyFrom.Thumbnail;
                this.LinearKeySource = myCopyFrom.LinearKeySource;
            }
        }

        public bool Equals(Source other)
        {
            if (other == null)
                return false;
            else if (this.routerID != other.routerID)
                return false;
            else if (this.routerInput != other.routerInput)
                return false;
            else if (this.inputConfigID != other.inputConfigID)
                return false;
            else if (this.preferredTreatmentID != other.preferredTreatmentID)
                return false;
            else if (this.preferredLayerID != other.preferredLayerID)
                return false;
            else if (this.thumbnail != other.thumbnail)
                return false;
            else if (this.linearKeySource != other.linearKeySource)
                return false;
            else
                return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Source source = obj as Source;
            if (source == null)
                return false;
            else
                return this.Equals(source);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Source source1, Source source2)
        {
            if (((object)source1 == null) || ((object)source2) == null)
                return Object.Equals(source1, source2);

            return source1.Equals(source2);
        }

        public static bool operator !=(Source source1, Source source2)
        {
            if (((object)source1 == null) || ((object)source2) == null)
                return !Object.Equals(source1, source2);

            return !source1.Equals(source2);
        }
    }
}
