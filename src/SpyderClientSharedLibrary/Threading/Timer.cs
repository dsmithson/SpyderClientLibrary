using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder.Client.Threading
{
    public delegate void TimerCallback(object state);

    public class Timer
    {
        private readonly object timerTaskLock = new object();
        private TaskCompletionSource<bool> timerTask;
        private TimerCallback callback;
        private object state;
        private int period;

        public Timer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
            : this(callback, state, (int)dueTime.TotalMilliseconds, (int)period.TotalMilliseconds)
        {
        }

        public Timer(TimerCallback callback, object state, int dueTime, int period)
        {
            this.callback = callback;
            this.state = state;

            Change(dueTime, period);
        }

        public void Change(TimeSpan dueTime, TimeSpan period)
        {
            Change((int)dueTime.TotalMilliseconds, (int)period.TotalMilliseconds);
        }

        public void Change(int dueTime, int period)
        {
            //Stop existing countdown (if any)
            lock (timerTaskLock)
            {
                if (timerTask != null)
                {
                    timerTask.TrySetResult(false);
                    timerTask = null;
                }
            }

            //Setup for next run
            this.period = period;
            if (dueTime >= 0)
            {
                SetupForNextTimerEvent(dueTime);
            }
        }

        private void SetupForNextTimerEvent(int delay)
        {
            TaskCompletionSource<bool> tcs;
            lock (timerTaskLock)
            {
                timerTask = new TaskCompletionSource<bool>();
                timerTask.Task.ContinueWith(timer_elapsed);

                tcs = timerTask;
            }

            //Setup timer event 
            Task.Delay(delay).ContinueWith((result) =>
                {
                    if (tcs != null)
                    {
                        lock (timerTaskLock)
                        {
                            //Ensure the timer reference hasn't been switched  out, due to a change
                            if (tcs == timerTask)
                            {
                                timerTask.TrySetResult(true);
                                timerTask = null;
                            }
                        }
                    }
                });
        }

        private void timer_elapsed(Task<bool> taskResult)
        {
            if (taskResult.Result == true)
            {
                //Raise callback
                callback(state);

                //Register for next periodic signaling time
                if (period >= 0)
                {
                    SetupForNextTimerEvent(period);
                }
            }
        }

        public void Dispose()
        {
            lock (timerTaskLock)
            {
                if (timerTask != null)
                {
                    //Cancel the internal task
                    timerTask.TrySetResult(false);
                    timerTask = null;
                }
            }
        }
    }
}
