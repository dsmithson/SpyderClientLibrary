using System;

namespace Spyder.Client.Diagnostics
{
    public class TraceMessage
    {
        public TracingLevel Level { get; set; }

        public string Message { get; set; }

        public object Sender { get; set; }

        public DateTime LogTime { get; set; }

        public TraceMessage()
        {
            LogTime = DateTime.Now;
        }
    }
}
