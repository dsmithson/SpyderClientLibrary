using Spyder.Client.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Net.Notifications
{
    public static class DiagnosticMessageTypeInfo
    {
        public static bool IsDiagnosticMessage(ServerEventType eventType)
        {
            return GetTracingLevelOrNull(eventType).HasValue;
        }

        public static TracingLevel GetTracingLevel(ServerEventType eventType)
        {
            var response = GetTracingLevelOrNull(eventType);
            if (response.HasValue)
                return response.Value;
            else
                throw new InvalidOperationException(string.Format("Server event type provided ({0}) is not a valid tracing level type", eventType));
        }

        private static TracingLevel? GetTracingLevelOrNull(ServerEventType eventType)
        {
            if (eventType == ServerEventType.Information)
                return TracingLevel.Information;
            else if (eventType == ServerEventType.Warning)
                return TracingLevel.Warning;
            else if (eventType == ServerEventType.Success)
                return TracingLevel.Success;
            else if (eventType == ServerEventType.Error || eventType == ServerEventType.Failure)
                return TracingLevel.Error;
            else
                return null;
        }
    }
}
