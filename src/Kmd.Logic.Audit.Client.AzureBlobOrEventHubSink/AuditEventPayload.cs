using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    /// <summary>
    /// This class handles methods related to Audit event payload
    /// </summary>
    public static class AuditEventPayload
    {
        public const string MessageTemplate = "Event details for event id {0} can be found at {1}";

        /// <summary>
        /// This method checks if audit event payload size is more than the limit or not
        /// </summary>
        /// <param name="textFormatter">Formatter</param>
        /// <param name="logEvent">Log event</param>
        /// <param name="eventSizeLimit">Event size limit</param>
        /// <returns>Returns if message size is greater than the limit</returns>
        public static bool DoesAuditEventPayloadExceedLimit(ITextFormatter textFormatter, LogEvent logEvent, int eventSizeLimit)
        {
            string content;
            using (var render = new StringWriter())
            {
                textFormatter.Format(logEvent, render);
                content = render.ToString();
            }

            if (Encoding.UTF8.GetByteCount(content) > eventSizeLimit)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Transform the logevent to provide different message with blob url property added
        /// </summary>
        /// <param name="logEvent">Log event</param>
        /// <param name="blobUrl">Blob url in string</param>
        /// <returns>New log event after message transformation</returns>
        public static LogEvent AuditEventMessageTransformation(LogEvent logEvent, string blobUrl)
        {
            IList<LogEventProperty> properties = new List<LogEventProperty>();
            foreach (var property in logEvent.Properties)
            {
                LogEventProperty logEventProperty = new LogEventProperty(property.Key, property.Value);
                properties.Add(logEventProperty);
            }

            LogEvent newLogEvent = new LogEvent(
                logEvent.Timestamp,
                logEvent.Level,
                logEvent.Exception,
                new MessageTemplate(string.Format(AuditEventPayload.MessageTemplate, logEvent.Properties["_EventId"], blobUrl), logEvent.MessageTemplate.Tokens),
                properties);

            return newLogEvent;
        }
    }
}
