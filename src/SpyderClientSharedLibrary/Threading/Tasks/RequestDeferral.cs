using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Threading.Tasks
{
    /// <summary>
    /// Token provided to asynchronous callers to help serialize asynchronous operations.
    /// </summary>
    public class RequestDeferral
    {
        private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public void Complete()
        {
            tcs.TrySetResult(true);
        }

        public Task WaitForCompletedAsync()
        {
            return tcs.Task;
        }

        public static async Task WaitForAllCompletedAsync(IEnumerable<RequestDeferral> requests)
        {
            if(requests != null)
            {
                var tasks = requests.Select(r => r.tcs.Task).ToList();
                if(tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                }
            }
        }
    }
}
