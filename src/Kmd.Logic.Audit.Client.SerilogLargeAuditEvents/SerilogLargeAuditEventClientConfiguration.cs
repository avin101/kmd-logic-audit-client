using System;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public class SerilogLargeAuditEventClientConfiguration
    {
        /// <summary>
        /// Gets or sets a value that will be tagged on every event to make
        /// it clear what the source of the event was.
        /// </summary>
        public string EventSource { get; set; }

        /// <summary>
        /// Gets or sets the Azure EventHubs connection string.
        /// </summary>
        public string EventhubConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the Azure Blob ConnectionString.
        /// </summary>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the Azure Blob account name.
        /// </summary>
        public string StorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets the Azure Blob container name.
        /// </summary>
        public string StorageContainerName { get; set; }

        /// <summary>
        /// Gets or sets the the topic where audit events are ingested on Azure EventHubs.
        /// Normally just leave this as the default value of "audit".
        /// </summary>
        public string AuditEventTopic { get; set; } = "audit";

        public bool? EnrichFromLogContext { get; set; }

        /// <summary>
        /// Gets or sets the event size limit which decides whether message will be pushed to blob and event hub or only event hub
        /// </summary>
        public int EventSizeLimitinBytes { get; set; }
    }
}
