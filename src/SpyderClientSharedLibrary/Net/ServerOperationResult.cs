using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net
{
    public enum CommandExecutionPriority { Immediate, Background }

    /// <summary>
    /// Stores the results of a client request / operation
    /// </summary>
    public class ServerOperationResult
    {
        public ServerOperationResultCode Result { get; set; }

        /// <summary>
        /// Priority level the task actually executed at
        /// </summary>
        public CommandExecutionPriority ExecutionPriorityLevel { get; set; }
        
        public List<string> ResponseData { get; set; } = new List<string>();

        public ServerOperationResult(ServerOperationResultCode result = ServerOperationResultCode.Success)
        {
            this.Result = result;
        }
    }
}
