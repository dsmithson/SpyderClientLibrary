using Spyder.Client.Common;

namespace Spyder.Client.FunctionKeys
{
    public class BackupSourceFunctionKey : RelativeFunctionKey
    {
        private string sourceName;
        public string SourceName
        {
            get { return sourceName; }
            set
            {
                if (sourceName != value)
                {
                    sourceName = value;
                    OnPropertyChanged();
                }
            }
        }

        private int sourceLookupID;
        public int SourceLookupID
        {
            get { return sourceLookupID; }
            set
            {
                if (sourceLookupID != value)
                {
                    sourceLookupID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string backupSourceName;
        public string BackupSourceName
        {
            get { return backupSourceName; }
            set
            {
                if (backupSourceName != value)
                {
                    backupSourceName = value;
                    OnPropertyChanged();
                }
            }
        }

        private SourceBackupType backupType;
        public SourceBackupType BackupType
        {
            get { return backupType; }
            set
            {
                if (backupType != value)
                {
                    backupType = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void CopyFrom(IRegister copyFrom)
        {
            base.CopyFrom(copyFrom);

            var myCopyFrom = copyFrom as BackupSourceFunctionKey;
            if (myCopyFrom != null)
            {
                this.SourceName = myCopyFrom.SourceName;
                this.SourceLookupID = myCopyFrom.SourceLookupID;
                this.BackupSourceName = myCopyFrom.BackupSourceName;
                this.BackupType = myCopyFrom.BackupType;
            }
        }
    }
}
