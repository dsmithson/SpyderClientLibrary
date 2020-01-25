using Knightware.Threading.Tasks;
using System.Threading.Tasks;

namespace Spyder.Client.Diagnostics
{
    public delegate void TracingLevelChangedHandler(TracingLevel newTracingLevel);
    public delegate void TraceMessageHandler(TraceMessage message);

    public static class TraceQueue
    {
        private const int maxMessageQueueCount = 100;
        private static readonly AsyncListProcessor<TraceMessage> messageQueue;

        /// <summary>
        /// Event raised when a new Trace message is generated
        /// </summary>
        public static event TraceMessageHandler TraceMessageRaised;
        //private static void OnTraceMessageRaised(TraceMessage message)
        //{
        //    if (TraceMessageRaised != null)
        //    {
        //        TraceMessageRaised(message);
        //    }
        //}

        /// <summary>
        /// Event raised when the tracing level has changed
        /// </summary>
        public static event TracingLevelChangedHandler TracingLevelChanged;
        private static void OnTracingLevelChanged(TracingLevel newTracingLevel)
        {
            TracingLevelChanged?.Invoke(newTracingLevel);
        }

        private static TracingLevel tracingLevel = Diagnostics.TracingLevel.Success;
        public static TracingLevel TracingLevel
        {
            get { return tracingLevel; }
            set
            {
                if (tracingLevel != value)
                {
                    tracingLevel = value;
                    OnTracingLevelChanged(value);
                }
            }
        }

        static TraceQueue()
        {
            //Initialize our message queue
            messageQueue = new AsyncListProcessor<TraceMessage>(ProcessQueue, maxQueueCount: maxMessageQueueCount);
            messageQueue.StartupAsync().Wait();
        }

        #region Trace Overloads

        public static void Trace(TracingLevel level, string message, params object[] args)
        {
            Trace(null, level, message, args);
        }

        public static void Trace(object sender, TracingLevel level, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            Trace(new TraceMessage()
            {
                Level = level,
                Sender = sender,
                Message = message,
            });
        }

        public static void Trace(TraceMessage message)
        {
            if (message != null && message.Level <= tracingLevel && TraceMessageRaised != null)
                messageQueue.Add(message);
        }

        #endregion

        private static Task ProcessQueue(AsyncListProcessorItemEventArgs<TraceMessage> args)
        {
            if (args != null && args.Item != null && TraceMessageRaised != null)
            {
                TraceMessageRaised(args.Item);
            }
            return Task.FromResult(true);
        }
    }
}
