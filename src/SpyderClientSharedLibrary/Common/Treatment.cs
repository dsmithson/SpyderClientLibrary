using Knightware.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Common
{
    public class Treatment : KeyFrame, IRegister
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

        private bool isSizeEnabled;
        public bool IsSizeEnabled
        {
            get { return isSizeEnabled; }
            set
            {
                if (isSizeEnabled != value)
                {
                    isSizeEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isHPositionEnabled;
        public bool IsHPositionEnabled
        {
            get { return isHPositionEnabled; }
            set
            {
                if (isHPositionEnabled != value)
                {
                    isHPositionEnabled = value;
                    OnPropertyChanged("IsPositionEnabled");
                    OnPropertyChanged();
                }
            }
        }

        private bool isVPositionEnabled;
        public bool IsVPositionEnabled
        {
            get { return isVPositionEnabled; }
            set
            {
                if (isVPositionEnabled != value)
                {
                    isVPositionEnabled = value;
                    OnPropertyChanged("IsPositionEnabled");
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPositionEnabled
        {
            get { return isVPositionEnabled || isHPositionEnabled; }
        }

        private bool isBorderEnabled;
        public bool IsBorderEnabled
        {
            get { return isBorderEnabled; }
            set
            {
                if (isBorderEnabled != value)
                {
                    isBorderEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isShadowEnabled;
        public bool IsShadowEnabled
        {
            get { return isShadowEnabled; }
            set
            {
                if (isShadowEnabled != value)
                {
                    isShadowEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isCloneEnabled;
        public bool IsCloneEnabled
        {
            get { return isCloneEnabled; }
            set
            {
                if (isCloneEnabled != value)
                {
                    isCloneEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isCropEnabled;
        public bool IsCropEnabled
        {
            get { return isCropEnabled; }
            set
            {
                if (isCropEnabled != value)
                {
                    isCropEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isAspectRatioOffsetEnabled;
        public bool IsAspectRatioOffsetEnabled
        {
            get { return isAspectRatioOffsetEnabled; }
            set
            {
                if (isAspectRatioOffsetEnabled != value)
                {
                    isAspectRatioOffsetEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isPanZoomEnabled;
        public bool IsPanZoomEnabled
        {
            get { return isPanZoomEnabled; }
            set
            {
                if (isPanZoomEnabled != value)
                {
                    isPanZoomEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isDurationEnabled;
        public bool IsDurationEnabled
        {
            get { return isDurationEnabled; }
            set
            {
                if (isDurationEnabled != value)
                {
                    isDurationEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public Treatment()
        {
        }

        public Treatment(IRegister copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public Treatment(KeyFrame copyFrom)
        {
            CopyFrom(copyFrom);
        }

        public virtual void CopyFrom(IRegister copyFrom)
        {
            if (copyFrom is KeyFrame)
            {
                CopyFrom((KeyFrame)copyFrom);
            }
            else
            {
                Register.Copy(copyFrom, this);
            }
        }

        public override void CopyFrom(KeyFrame copyFrom)
        {
            //Update keyframe properties
            base.CopyFrom(copyFrom);

            //Update reister properties
            var myRegister = copyFrom as IRegister;
            if (myRegister != null)
            {
                Register.Copy(myRegister, this);
            }

            //Update treatment properties
            var myCopyFrom = copyFrom as Treatment;
            if (myCopyFrom != null)
            {
                this.ID = myCopyFrom.ID;
                this.IsAspectRatioOffsetEnabled = myCopyFrom.IsAspectRatioOffsetEnabled;
                this.IsBorderEnabled = myCopyFrom.IsBorderEnabled;
                this.IsCloneEnabled = myCopyFrom.IsCloneEnabled;
                this.IsCropEnabled = myCopyFrom.IsCropEnabled;
                this.IsDurationEnabled = myCopyFrom.IsDurationEnabled;
                this.IsPanZoomEnabled = myCopyFrom.IsPanZoomEnabled;
                this.IsHPositionEnabled = myCopyFrom.IsHPositionEnabled;
                this.IsVPositionEnabled = myCopyFrom.IsVPositionEnabled;
                this.IsShadowEnabled = myCopyFrom.IsShadowEnabled;
                this.IsSizeEnabled = myCopyFrom.IsSizeEnabled;
            }
        }

        #region IRegister Implementation

        private int lookupID;
        public int LookupID
        {
            get { return lookupID; }
            set
            {
                if (lookupID != value)
                {
                    lookupID = value;
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

        public int PageIndex
        {
            get { return RegisterID % 1000; }
        }

        private Color registerColor;
        public Color RegisterColor
        {
            get { return registerColor; }
            set
            {
                if (registerColor != value)
                {
                    registerColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool registerColorDefined;
        public bool RegisterColorDefined
        {
            get { return registerColorDefined; }
            set
            {
                if (registerColorDefined != value)
                {
                    registerColorDefined = value;
                    OnPropertyChanged();
                }
            }
        }

        private int registerID;
        public int RegisterID
        {
            get { return registerID; }
            set
            {
                if (registerID != value)
                {
                    registerID = value;
                    OnPropertyChanged();
                }
            }
        }

        private RegisterType type = RegisterType.Treatment;
        public virtual RegisterType Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}
