using Spyder.Client.Common;
using System.Collections.Generic;

namespace Spyder.Client.FunctionKeys
{
    public abstract class RelativeFunctionKey : FunctionKey, IRelativeFunctionKey
    {
        private bool isAbsolute;
        public bool IsAbsolute
        {
            get { return isAbsolute; }
            set
            {
                if (isAbsolute != value)
                {
                    isAbsolute = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<int> absoluteLayerIDs = new List<int>();
        public List<int> AbsoluteLayerIDs
        {
            get { return absoluteLayerIDs; }
            set
            {
                if (absoluteLayerIDs != value)
                {
                    absoluteLayerIDs = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as IRelativeFunctionKey;
            if (myCopyFrom != null)
            {
                this.IsAbsolute = myCopyFrom.IsAbsolute;
                this.AbsoluteLayerIDs = new List<int>(myCopyFrom.AbsoluteLayerIDs);
            }
        }
    }
}
