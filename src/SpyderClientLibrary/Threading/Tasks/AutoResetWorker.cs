using Spyder.Client.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder.Client.Threading.Tasks
{
    /// <summary>
    /// Worker class wrapped around an AsyncAutoResetEvent which provisions a worker thread to run when the AutoResetEvent is signalled
    /// </summary>
    public class AutoResetWorker
    {
        private AsyncAutoResetEvent asyncResetEvent;
        private bool isWorkerRunning;
        private object workerState;

        public bool IsRunning { get; private set; }

        /// <summary>
        /// When set, will automatically trigger the update method at a defined interval if not triggered manually.  Set to Timeout.InfiniteTimeSpan to disable.
        /// </summary>
        public TimeSpan PeriodicSignallingTime
        {
            get { return periodicSignallingTime; }
            set
            {
                periodicSignallingTime = value;
            }
        }
        private TimeSpan periodicSignallingTime = Timeout.InfiniteTimeSpan;

        /// <summary>
        /// Executed before each iteration of the worker to determine if it should still be runing
        /// </summary>
        private Func<bool> checkForContinueMethod;

        /// <summary>
        /// Worker method run upon each iteration
        /// </summary>
        private Func<object, Task> workerMethod;

        public bool Startup(Func<object, Task> workerMethod, object workerState, Func<bool> checkForContinueMethod = null)
        {
            Shutdown();

            if (workerMethod == null)
                throw new ArgumentException("WorkerMethod cannot be null", "workerMethod");

            IsRunning = true;

            this.workerMethod = workerMethod;
            this.workerState = workerState;
            this.checkForContinueMethod = checkForContinueMethod;

            asyncResetEvent = new AsyncAutoResetEvent();
            Task.Factory.StartNew(() => AsyncWorker(), TaskCreationOptions.LongRunning);
            return true;
        }

        public void Shutdown()
        {
            IsRunning = false;

            if (isWorkerRunning)
            {
                if (asyncResetEvent != null)
                {
                    asyncResetEvent.Set();
                }

                while (isWorkerRunning)
                {
                    Task.Delay(10).Wait();
                }
            }

            workerMethod = null;
            checkForContinueMethod = null;
            asyncResetEvent = null;
        }

        /// <summary>
        /// Signals the AutoResetEvent
        /// </summary>
        public void Set()
        {
            if (IsRunning)
            {
                if (asyncResetEvent != null)
                    asyncResetEvent.Set();
            }
        }

        private async void AsyncWorker()
        {
            isWorkerRunning = true;

            while (IsRunning)
            {
                //Wait to be signalled
                await asyncResetEvent.WaitAsync(PeriodicSignallingTime);

                //Ensure we should still be running
                if (!IsRunning)
                    break;

                //Check if we should be running now.  If so, we wait, but don't break out of the loop
                if (checkForContinueMethod != null && !checkForContinueMethod())
                    continue;

                try
                {
                    await workerMethod(workerState);
                }
                catch (Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while running worker method: {1}\r\n\r\n{2}", ex.GetType().Name, ex.Message, ex.StackTrace);
                }
            }

            //Signal completion
            isWorkerRunning = false;
        }
    }
}
