using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Azure.EventHubs;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    /// <summary>
    /// This class implements methods which will help before using the actual eventhub
    /// </summary>
    public class AzureEventHubServiceHelper : IAzureEventHubServiceHelper
    {
        /// <summary>
        /// Prepares the message content to be pushed to event hub
        /// </summary>
        /// <param name="textFormatter">Text Formatter to format content</param>
        /// <param name="logEvent">Log event sent by client</param>
        /// <returns>Event data which is pushed to event hub</returns>
        public EventData PrepareEventHubMessageContent(ITextFormatter textFormatter, LogEvent logEvent)
        {
            byte[] body;
            using (var render = new StringWriter())
            {
                textFormatter.Format(logEvent, render);
                body = Encoding.UTF8.GetBytes(render.ToString());
            }

            var eventHubData = new EventData(body);
            eventHubData.Properties.Add("Type", "SerilogEvent");
            eventHubData.Properties.Add("Level", logEvent.Level.ToString());
            return eventHubData;
        }
    }
}
