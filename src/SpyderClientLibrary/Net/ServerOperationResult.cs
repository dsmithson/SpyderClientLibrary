using System.Collections.Generic;
using System.Linq;

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

        private List<string> responseData;
        public List<string> ResponseData
        {
            get
            {
                if (responseData == null)
                    UpdateResponseData();

                return responseData;
            }
        }

        public string ResponseRaw { get; set; }

        public ServerOperationResult(ServerOperationResultCode result = ServerOperationResultCode.Success)
        {
            this.Result = result;
        }

        public void UpdateResponseData()
        {
            if (string.IsNullOrEmpty(ResponseRaw))
            {
                responseData = new List<string>();
            }
            else
            {
                //Build our split up response data and cache for future requests
                responseData = ResponseRaw
                    .Split(' ')
                    .Select(s => SpyderUdpClient.DecodeSpyderParameter(s))
                    .ToList();
            }
        }
    }
}
