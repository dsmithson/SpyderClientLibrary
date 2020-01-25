using System;

namespace Spyder.Client.Images
{
    public class QFTThumbnailIdentifier : IEquatable<QFTThumbnailIdentifier>
    {
        public string ServerIP
        {
            get;
            private set;
        }

        public string FileName
        {
            get;
            private set;
        }

        public QFTThumbnailIdentifier(string serverIP, string fileName)
        {
            if (string.IsNullOrEmpty(serverIP))
                throw new ArgumentException("ServerIP cannot be null", "serverIP");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("FileName cannot be null", "fileName");

            this.ServerIP = serverIP;
            this.FileName = fileName;
        }

        public virtual bool Equals(QFTThumbnailIdentifier other)
        {
            return other.FileName == this.FileName && other.ServerIP == this.ServerIP;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as QFTThumbnailIdentifier;
            if (compareTo == null)
                return false;

            return compareTo.FileName == this.FileName && compareTo.ServerIP == this.ServerIP;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ServerIP, FileName);
        }
    }
}
