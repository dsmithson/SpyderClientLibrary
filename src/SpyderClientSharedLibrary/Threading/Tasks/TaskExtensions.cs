using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static async Task<bool> AllSuccess( params Task<bool>[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
                return false;

            await Task.WhenAll(tasks);

            return tasks.All(task => task.Exception == null && task.Result);
        }
    }
}
