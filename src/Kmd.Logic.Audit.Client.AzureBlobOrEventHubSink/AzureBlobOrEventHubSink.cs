using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.EventHubs;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AzureBlobOrEventHubSink : ILogEventSink
    {
        private readonly ITextFormatter textFormatter;
        private readonly BlobServiceClient blobServiceClient;
        private readonly IAzureBlobServiceHelper azureBlobServiceHelper;
        private readonly IAzureBlobServiceProvider azureBlobServiceProvider;
        private readonly string storageContainerName;
        private readonly EventHubClient eventHubClient;
        private readonly IAzureEventHubServiceHelper azureEventhubServiceHelper;
        private readonly IAzureEventHubServiceProvider azureEventhubServiceProvider;
        private string storageBlobName;
        private int eventSizeLimit;

        public AzureBlobOrEventHubSink(
            BlobServiceClient blobServiceClient,
            ITextFormatter textFormatter,
            EventHubClient eventHubClient,
            int eventSizeLimit,
            string storageContainerName = null,
            string storageBlobName = null,
            IAzureBlobServiceHelper azureBlobServiceHelper = null,
            IAzureBlobServiceProvider azureBlobServiceProvider = null,
            IAzureEventHubServiceHelper azureEventhubServiceHelper = null,
            IAzureEventHubServiceProvider azureEventhubServiceProvider = null)
        {
            if (string.IsNullOrEmpty(storageContainerName))
            {
                storageContainerName = "logs";
            }

            if (string.IsNullOrEmpty(storageBlobName))
            {
                storageBlobName = "log";
            }

            if (eventSizeLimit == 0)
            {
                eventSizeLimit = 256 * 1024;
            }

            this.textFormatter = textFormatter;
            this.blobServiceClient = blobServiceClient;
            this.storageContainerName = storageContainerName;
            this.storageBlobName = storageBlobName;
            this.azureBlobServiceHelper = azureBlobServiceHelper ?? new AzureBlobServiceHelper();
            this.azureBlobServiceProvider = azureBlobServiceProvider ?? new AzureBlobServiceProvider();
            this.azureEventhubServiceHelper = azureEventhubServiceHelper ?? new AzureEventHubServiceHelper();
            this.azureEventhubServiceProvider = azureEventhubServiceProvider ?? new AzureEventHubServiceProvider();
            this.eventHubClient = eventHubClient;
            this.eventSizeLimit = eventSizeLimit;
        }

        public void Emit(LogEvent logEventPrm)
        {
            string blobUrl = string.Empty;
            LogEvent logEvent = logEventPrm;
            if (AuditEventPayload.DoesAuditEventPayloadExceedLimit(this.textFormatter, logEventPrm, this.eventSizeLimit))
            {
                blobUrl = this.UploadToBlob(logEventPrm);
                logEvent = AuditEventPayload.AuditEventMessageTransformation(logEventPrm, blobUrl);
            }

            this.PushToEventhub(logEvent);
        }

        /// <summary>
        /// Handles uploading to Azure blob
        /// </summary>
        /// <param name="logEvent">Log event</param>
        /// <returns>blob url</returns>
        private string UploadToBlob(LogEvent logEvent)
        {
            var content = this.azureBlobServiceHelper.PrepareBlobContentForUpload(this.textFormatter, logEvent);

            // Get Event Id value and use it as blob name
            LogEventPropertyValue eventIdValue;
            logEvent.Properties.TryGetValue("_EventId", out eventIdValue);
            if (eventIdValue != null)
            {
                this.storageBlobName = eventIdValue.ToString();
            }

            var blobUrl = this.azureBlobServiceProvider.UploadBlob(this.blobServiceClient, this.storageContainerName, this.storageBlobName, content);
            return blobUrl;
        }

        private void PushToEventhub(LogEvent logEvent)
        {
            var eventHubData = this.azureEventhubServiceHelper.PrepareEventHubMessageContent(this.textFormatter, logEvent);
            this.azureEventhubServiceProvider.PostMessage(this.eventHubClient, eventHubData);
        }
    }
}
