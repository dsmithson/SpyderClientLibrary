using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder.Client.Threading.Tasks
{
    public class AsyncAutoResetEvent
    {
        private readonly static Task<bool> completed = Task.FromResult(true);
        private readonly List<AsyncAutoResetEventState> waits = new List<AsyncAutoResetEventState>();
        private bool signaled; 

        public Task<bool> WaitAsync()
        {
            return WaitAsync(Timeout.Infinite);
        }

        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            return WaitAsync((int)timeout.TotalMilliseconds);
        }

        public Task<bool> WaitAsync(int timeout)
        {
            lock (waits)
            {
                if (signaled)
                {
                    signaled = false;
                    return completed;
                }
                else
                {
                    var tcs = new AsyncAutoResetEventState()
                    {
                        TaskCompletion = new TaskCompletionSource<bool>()
                    };
                    if (timeout != Timeout.Infinite)
                    {
                        tcs.TimeoutTimer = new Timer(OnTimeoutTimer_Elapsed, tcs, timeout, Timeout.Infinite);
                    }
                    waits.Add(tcs);

                    return tcs.TaskCompletion.Task;
                }
            }
        }

        public void Set()
        {
            AsyncAutoResetEventState toRelease = null;
            lock (waits)
            {
                if (waits.Count > 0)
                {
                    toRelease = waits[0];
                    waits.RemoveAt(0);
                }
                else if (!signaled)
                {
                    signaled = true;
                }
            }

            if (toRelease != null)
            {
                toRelease.TaskCompletion.SetResult(true);

                if (toRelease.TimeoutTimer != null)
                {
                    toRelease.TimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    toRelease.TimeoutTimer.Dispose();
                }
            }
        }

        private void OnTimeoutTimer_Elapsed(object state)
        {
            var tcs = (AsyncAutoResetEventState)state;
            bool wasInQueue = false;
            lock (waits)
            {
                if (waits.Contains(tcs))
                {
                    waits.Remove(tcs);
                    wasInQueue = true;
                }
            }
            if (wasInQueue)
            {
                tcs.TaskCompletion.SetResult(false);

                if (tcs.TimeoutTimer != null)
                {
                    tcs.TimeoutTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    tcs.TimeoutTimer.Dispose();
                }
            }
        }

        private class AsyncAutoResetEventState
        {
            public TaskCompletionSource<bool> TaskCompletion { get; set; }
            public Timer TimeoutTimer { get; set; }
        }
    }
}
