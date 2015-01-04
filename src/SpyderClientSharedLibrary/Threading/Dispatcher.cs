using Spyder.Client.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spyder.Client.Threading
{
    public class Dispatcher
    {
        private readonly SynchronizationContext context;

        public static Dispatcher Current
        {
            get { return new Dispatcher(); }
        }

        public bool InvokeRequired
        {
            get { return context != null && context != SynchronizationContext.Current; }
        }

        public Dispatcher()
        {
            context = SynchronizationContext.Current;
        }

        public Dispatcher(SynchronizationContext context)
        {
            this.context = context;
        }

        //public void Invoke(Action action)
        //{
        //    //Wrap into a dummy bool returning function
        //    Invoke<bool>(() => 
        //        {
        //            action();
        //            return true;
        //        });
        //}

        //public T Invoke<T>(Func<T> func)
        //{
        //    return BeginInvoke(func).Result;
        //}

        public Task BeginInvoke(Action action)
        {
            return BeginInvoke<bool>(() =>
                {
                    action();
                    return true;
                });
        }

        public Task<T> BeginInvoke<T>(Func<T> func)
        {
            try
            {
                if (context == null)
                {
                    //Run synchronously outside of any sync context
                    return Task.FromResult(func());
                }
                else if (context == SynchronizationContext.Current)
                {
                    //Already in our context, so execute directly
                    return Task.FromResult(func());
                }
                else
                {
                    //Send to sync context
                    try
                    {
                        var tcs = new TaskCompletionSource<T>();
                        context.Post((state) =>
                            {
                                T result = func();
                                tcs.TrySetResult(result);
                            }, null);

                        return tcs.Task;
                    }
                    catch (Exception ex)
                    {
                        //When a window is closing and a beginInvoke comes in, a System.Exception will be raised with a note saying
                        //that this exception can be safely ignored if handled.
                        TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to invoke a method: {1}", ex.GetType().Name, ex.Message);
                        return Task.FromResult<T>(default(T));
                    }
                }
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while invoking an action: {1}", ex.GetType().Name, ex.Message);
                return Task.FromResult(default(T));
            }
        }
    }
}
