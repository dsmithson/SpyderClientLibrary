using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spyder.Client.Threading.Tasks
{
    public class AsyncSemaphore
    {
        private readonly static Task completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> waiters = new Queue<TaskCompletionSource<bool>>();
        private int currentCount;

        public AsyncSemaphore(int initialCount)
        {
            currentCount = initialCount;
        }

        public Task WaitAsync()
        {
            lock (waiters)
            {
                if (currentCount > 0)
                {
                    //Resource available, return a completed task immediately
                    currentCount--;
                    return completed;
                }
                else
                {
                    //Return a task for this item to be completed when a resource is available
                    var waiter = new TaskCompletionSource<bool>();
                    waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (waiters)
            {
                if (waiters.Count > 0)
                {
                    //Get next waiter to signal
                    toRelease = waiters.Dequeue();
                }
                else
                {
                    //No waiters, so increment our resource count
                    currentCount++;
                }
            }

            //Release next waiter, if available
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
}
