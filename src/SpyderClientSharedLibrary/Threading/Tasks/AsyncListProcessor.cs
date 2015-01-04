using Spyder.Client.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Threading.Tasks
{
    /// <summary>
    /// Manages a collection of items which can be added to in a thread-safe mannor, and will be processed sequentially
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncListProcessor<T>
    {
        private readonly AutoResetWorker worker = new AutoResetWorker();
        private readonly Queue<T> internalQueue = new Queue<T>();
        private Func<AsyncListProcessorItemEventArgs<T>, Task> processItem;
        private Func<bool> checkForContinueMethod;

        public bool IsRunning { get; private set; }

        /// <summary>
        /// Maximimum items to allow to be in the queue, or 0 for no limit
        /// </summary>
        public int MaximumQueueCount { get; set; }
        
        public AsyncListProcessor(Func<AsyncListProcessorItemEventArgs<T>, Task> processItem, Func<bool> checkForContinueMethod = null)
        {
            if (processItem == null)
                throw new ArgumentException("ProcessItem may not be null", "processItem");

            this.processItem = processItem;
            this.checkForContinueMethod = checkForContinueMethod;
        }
        
        public bool Startup()
        {
            Shutdown();
            IsRunning = true;

            Func<bool> continueWorking = () =>
            {
                if (!IsRunning)
                    return false;
                else if (checkForContinueMethod != null)
                    return checkForContinueMethod();
                else
                    return true;
            };

            if (!worker.Startup(collectionProcessorWorker, null, continueWorking))
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Failed to startup worker.  Shutting down.");
                Shutdown();
                return false;
            }

            return true;
        }

        public void Shutdown()
        {
            IsRunning = false;

            if (worker != null)
            {
                worker.Shutdown();
            }

            internalQueue.Clear();
        }

        public void Add(T newItem)
        {
            if (newItem == null)
                return;

            lock (internalQueue)
            {
                internalQueue.Enqueue(newItem);

                //Remove extra items
                if(MaximumQueueCount > 0 && internalQueue.Count > MaximumQueueCount)
                {
                    while(internalQueue.Count > MaximumQueueCount)
                    {
                        internalQueue.Dequeue();
                    }
                }
            }
            worker.Set();
        }

        public void AddRange(IEnumerable<T> newItems)
        {
            if (newItems == null)
                return;

            lock (internalQueue)
            {
                foreach (T newItem in newItems)
                {
                    internalQueue.Enqueue(newItem);
                }

                //Remove extra items
                if (MaximumQueueCount > 0 && internalQueue.Count > MaximumQueueCount)
                {
                    while (internalQueue.Count > MaximumQueueCount)
                    {
                        internalQueue.Dequeue();
                    }
                }
            }
            worker.Set();
        }

        private async Task collectionProcessorWorker(object state)
        {
            while (IsRunning && internalQueue.Count > 0)
            {
                try
                {
                    //De-Queue an item for processing
                    T item;
                    lock (internalQueue)
                    {
                        item = internalQueue.Dequeue();
                    }

                    //Process the current item
                    await ProcessSingleItem(item);
                }
                catch (Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while processing AsyncList Item: {1}", ex.GetType().Name, ex.Message);
                }
            }
        }

        private async Task ProcessSingleItem(T item)
        {
            try
            {
                //Process the current item
                var args = new AsyncListProcessorItemEventArgs<T>(
                    item,
                    () => Peek(),
                    (subItem) => ProcessSingleItem(subItem));

                await processItem(args);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while processing AsyncList Item: {1}", ex.GetType().Name, ex.Message);
            }
        }

        private T Peek()
        {
            lock (internalQueue)
            {
                if (internalQueue.Count == 0)
                    return default(T);

                return internalQueue.Peek();
            }
        }
    }
}
