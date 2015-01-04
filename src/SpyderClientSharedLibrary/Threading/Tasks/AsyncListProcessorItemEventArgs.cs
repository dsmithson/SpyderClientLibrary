using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Threading.Tasks
{
    public class AsyncListProcessorItemEventArgs<T>
    {
        private Func<T> peekMethod;
        private Func<T, Task> processItemMethod;

        public T Item { get; private set; }

        public AsyncListProcessorItemEventArgs(T item, Func<T> peekMethod, Func<T, Task> processItemMethod)
        {
            this.Item = item;
            this.peekMethod = peekMethod;
            this.processItemMethod = processItemMethod;
        }

        /// <summary>
        /// Gets the next item in the processor list, without removing that item
        /// </summary>
        public T Peek()
        {
            if (peekMethod == null)
                return default(T);

            return peekMethod();
        }

        /// <summary>
        /// Processes one or more items immediately, allowing them to be run in-line with the outer item being processed
        /// </summary>
        public async Task ProcessItems(IEnumerable<T> itemsToProcess)
        {
            foreach (T itemToProcess in itemsToProcess)
            {
                await ProcessItem(itemToProcess);
            }
        }

        /// <summary>
        /// Processes a provided item immediately, allowing it to be run in-line with the outer item being processed
        /// </summary>
        public Task ProcessItem(T itemToProcess)
        {
            if (processItemMethod == null)
                throw new InvalidOperationException("No handler is registered to handle the ProcessItem method");

            //Process the item now
            return processItemMethod(itemToProcess);
        }
    }
}
