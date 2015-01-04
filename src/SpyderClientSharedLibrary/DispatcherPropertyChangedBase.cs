using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Spyder.Client.Diagnostics;
using Spyder.Client.Threading;

namespace Spyder.Client
{
    /// <summary>
    /// Captures SynchronizationContext at the time PropertyChanged is subscribed to, and event invocations are dispatched automatically
    /// </summary>
    public class DispatcherPropertyChangedBase : INotifyPropertyChanged
    {
        private readonly List<Tuple<PropertyChangedEventHandler, Dispatcher>> subscribers = new List<Tuple<PropertyChangedEventHandler,Dispatcher>>();
        private readonly object subscribserLock = new object();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add 
            {
                lock(subscribserLock)
                    subscribers.Add(new Tuple<PropertyChangedEventHandler, Dispatcher>(value, new Dispatcher()));
            }
            remove 
            {
                lock (subscribserLock)
                {
                    var subscriber = subscribers.FirstOrDefault(s => s.Item1 == value);
                    if (subscriber != null)
                        subscribers.Remove(subscriber);
                    else
                        TraceQueue.Trace(this, TracingLevel.Warning, "object requesting de-registration does not appear to be registered");
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            List<Tuple<PropertyChangedEventHandler, Dispatcher>> delegateList;
            lock (subscribserLock)
            {
                delegateList = new List<Tuple<PropertyChangedEventHandler, Dispatcher>>(subscribers);
            }

            var args = new PropertyChangedEventArgs(propertyName);
            foreach (var subscriber in delegateList)
            {
                try
                {
                    if (subscriber.Item2.InvokeRequired)
                    {
                        subscriber.Item2.BeginInvoke(() => subscriber.Item1(this, args));
                    }
                    else
                    {
                        subscriber.Item1(this, args);
                    }
                }
                catch(Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while raising propertyChanged: {1}", ex.GetType().Name, ex.Message);
                }
            }
        }
    }
}
