using Spyder.Client.Common;
using System.Collections.Generic;

namespace Spyder.Client.Net.DrawingData
{
    public class DrawingRouter : Router
    {
        private List<int> crosspoints = new List<int>();
        public List<int> Crosspoints
        {
            get { return crosspoints; }
            set
            {
                if (crosspoints != value)
                {
                    crosspoints = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
