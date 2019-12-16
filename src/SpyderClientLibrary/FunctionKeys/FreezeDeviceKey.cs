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

        private FreezeKeyAction freeze;
        public FreezeKeyAction Freeze
        {
            get { return freeze; }
            set
            {
                if (freeze != value)
                {
                    freeze = value;
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
                this.Freeze = myCopyFrom.Freeze;
            }
        }
    }
}
