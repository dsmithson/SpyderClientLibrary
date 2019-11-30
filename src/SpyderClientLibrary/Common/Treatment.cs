using Knightware.Primitives;
using System;

namespace Spyder.Client.Common
{
    public class Treatment : KeyFrame, IRegister, IEquatable<Treatment>
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

        private bool isTransparencyEnabled;
        public bool IsTransparencyEnabled
        {
            get { return isTransparencyEnabled; }
            set
            {
                if (isTransparencyEnabled != value)
                {
                    isTransparencyEnabled = value;
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
            var kf = copyFrom as KeyFrame;
            if (kf != null)
            {
                CopyFrom((KeyFrame)copyFrom);
            }

            Register.Copy(copyFrom, this);
        }

        public override void CopyFrom(KeyFrame copyFrom)
        {
            Treatment treatment = copyFrom as Treatment;
            if (treatment != null)
            {
                CopyFrom(treatment);
            }
            else
            {
                //Update keyframe properties
                base.CopyFrom(copyFrom);

                //Update reister properties
                if (copyFrom is IRegister myRegister)
                {
                    Register.Copy(myRegister, this);
                }
            }
        }

        public virtual void CopyFrom(Treatment copyFrom)
        {
            if (copyFrom == null)
                return;

            //Update keyframe properties
            base.CopyFrom(copyFrom);

            //Update treatment properties
            if (copyFrom != null)
            {
                this.ID = copyFrom.ID;
                this.IsAspectRatioOffsetEnabled = copyFrom.IsAspectRatioOffsetEnabled;
                this.IsBorderEnabled = copyFrom.IsBorderEnabled;
                this.IsCloneEnabled = copyFrom.IsCloneEnabled;
                this.IsCropEnabled = copyFrom.IsCropEnabled;
                this.IsDurationEnabled = copyFrom.IsDurationEnabled;
                this.IsPanZoomEnabled = copyFrom.IsPanZoomEnabled;
                this.IsHPositionEnabled = copyFrom.IsHPositionEnabled;
                this.IsVPositionEnabled = copyFrom.IsVPositionEnabled;
                this.IsShadowEnabled = copyFrom.IsShadowEnabled;
                this.IsSizeEnabled = copyFrom.IsSizeEnabled;
                this.IsTransparencyEnabled = copyFrom.IsTransparencyEnabled;
            }

            //Update register properties
            Register.Copy(copyFrom, this);
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

        public override int GetHashCode()
        {
            return (((int)Type * 251) + RegisterID) * 251 + LookupID;
        }

        public bool Equals(Treatment other)
        {
            if (other == null)
                return false;
            else if (this.id != other.id)
                return false;
            else if (this.isAspectRatioOffsetEnabled != other.isAspectRatioOffsetEnabled)
                return false;
            else if (this.isBorderEnabled != other.isBorderEnabled)
                return false;
            else if (this.isCloneEnabled != other.isCloneEnabled)
                return false;
            else if (this.isDurationEnabled != other.isDurationEnabled)
                return false;
            else if (this.isPanZoomEnabled != other.isPanZoomEnabled)
                return false;
            else if (this.isHPositionEnabled != other.isHPositionEnabled)
                return false;
            else if (this.isVPositionEnabled != other.isVPositionEnabled)
                return false;
            else if (this.isShadowEnabled != other.isShadowEnabled)
                return false;
            else if (this.isSizeEnabled != other.isSizeEnabled)
                return false;
            else if (this.isTransparencyEnabled != other.isTransparencyEnabled)
                return false;
            else
                return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Treatment compare = obj as Treatment;
            if (compare == null)
                return false;
            else
                return this.Equals(compare);
        }

        public static bool operator ==(Treatment t1, Treatment t2)
        {
            if (t1 is null || t2 is null)
                return Object.Equals(t1, t2);

            return t1.Equals(t2);
        }

        public static bool operator !=(Treatment t1, Treatment t2)
        {
            if (t1 is null || t2 is null)
                return !Object.Equals(t1, t2);

            return !t1.Equals(t2);
        }
    }
}
