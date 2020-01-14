namespace Spyder.Client.FunctionKeys
{
    public enum FreezeKeyAction {  True, False, Toggle }

    public class FreezeLayerKey : RelativeFunctionKey
    {
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

            var myCopyFrom = copyFrom as FreezeLayerKey;
            if (myCopyFrom != null)
            {
                this.Freeze = myCopyFrom.Freeze;
            }
        }
    }
}
