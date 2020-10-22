using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    /// <summary>
    /// This interface provides required services to use the event hub
    /// </summary>
    public interface IAzureEventHubServiceProvider
    {
        /// <summary>
        /// Post message to event hub
        /// </summary>
        /// <param name="eventHubClient">Event hub client used to send message to eventhub</param>
        /// <param name="eventHubData">Data</param>
        void PostMessage(EventHubClient eventHubClient, EventData eventHubData);
    }
}
