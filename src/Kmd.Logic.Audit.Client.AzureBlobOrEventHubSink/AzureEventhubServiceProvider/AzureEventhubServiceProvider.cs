using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    /// <summary>
    /// This class provides implementation for services to use the event hub
    /// </summary>
    public class AzureEventHubServiceProvider : IAzureEventHubServiceProvider
    {
        /// <summary>
        /// Post message to event hub
        /// </summary>
        /// <param name="eventHubClient">Event hub client used to send message to eventhub</param>
        /// <param name="eventHubData">Data</param>
        public void PostMessage(EventHubClient eventHubClient, EventData eventHubData)
        {
            eventHubClient.SendAsync(eventHubData, Guid.NewGuid().ToString()).GetAwaiter().GetResult();
        }
    }
}
