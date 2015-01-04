using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spyder.Client.Common
{
    public class Source : Register
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
    }
}
