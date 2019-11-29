namespace Spyder.Client.Models
{
    public abstract class RenderObject : PropertyChangedBase
    {
        private int zIndex;
        public int ZIndex
        {
            get { return zIndex; }
            set
            {
                if (zIndex != value)
                {
                    zIndex = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
