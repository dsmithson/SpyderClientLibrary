using System;
using System.Collections.Generic;
using System.Text;

namespace Spyder.Client.Net
{
    public class DataIOProcessorStatus
    {
        /// <summary>
        /// Raw counter reading, which will be server set above 100 (to 101) when idle
        /// </summary>
        public int PercentCompleteRaw { get; set; }

        public string Message { get; set; }

        public int PercentComplete 
        { 
            get 
            { 
                return Math.Min(100, PercentCompleteRaw); 
            } 
        }

        /// <summary>
        /// Determines if Spyder is currently performing a background task
        /// </summary>
        public bool IsIdle
        {
            get
            {
                return PercentCompleteRaw == 101;
            }
        }

        public DataIOProcessorStatus()
        {

        }

        public DataIOProcessorStatus(int percentCompleteRaw, string message)
        {
            this.PercentCompleteRaw = percentCompleteRaw;
            this.Message = message;
        }
    }
}
