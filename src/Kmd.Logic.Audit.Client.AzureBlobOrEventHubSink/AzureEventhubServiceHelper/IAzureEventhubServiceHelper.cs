using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    /// <summary>
    /// This inerface provides methods which will help before using the actual eventhub
    /// </summary>
    public interface IAzureEventHubServiceHelper
    {
        /// <summary>
        /// Prepares the message content to be pushed to event hub
        /// </summary>
        /// <param name="textFormatter">Text Formatter to format content</param>
        /// <param name="logEvent">Log event sent by client</param>
        /// <returns>Event data which is pushed to event hub</returns>
        EventData PrepareEventHubMessageContent(ITextFormatter textFormatter, LogEvent logEvent);
    }
}
