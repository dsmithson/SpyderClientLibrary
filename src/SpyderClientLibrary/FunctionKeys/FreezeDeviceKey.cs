namespace Spyder.Client.FunctionKeys
{
    public class FreezeDeviceKey : RelativeFunctionKey
    {
        private bool setTopLayer;
        public bool SetTopLayer
        {
            get { return setTopLayer; }
            set
            {
                if (setTopLayer != value)
                {
                    setTopLayer = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool setBottomLayer;
        public bool SetBottomLayer
        {
            get { return setBottomLayer; }
            set
            {
                if (setBottomLayer != value)
                {
                    setBottomLayer = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isFreezeEnabled;
        public bool IsFreezeEnabled
        {
            get { return isFreezeEnabled; }
            set
            {
                if (isFreezeEnabled != value)
                {
                    isFreezeEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(Common.IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as FreezeDeviceKey;
            if (myCopyFrom != null)
            {
                this.SetTopLayer = myCopyFrom.SetTopLayer;
                this.SetBottomLayer = myCopyFrom.SetBottomLayer;
                this.IsFreezeEnabled = myCopyFrom.IsFreezeEnabled;
            }
        }
    }
}
