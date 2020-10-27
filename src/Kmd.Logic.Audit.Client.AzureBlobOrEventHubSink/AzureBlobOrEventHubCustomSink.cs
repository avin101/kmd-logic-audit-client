﻿using System;
using Azure.Storage.Blobs;
using Microsoft.Azure.EventHubs;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AzureBlobOrEventHubCustomSink : ILogEventSink
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly ITextFormatter textFormatter;
        private readonly EventHubClient eventHubClient;
        private readonly int eventSizeLimit;
        private readonly string storageContainerName;

        public AzureBlobOrEventHubCustomSink(
            BlobServiceClient blobServiceClient,
            ITextFormatter textFormatter,
            EventHubClient eventHubClient,
            int eventSizeLimit = 256 * 1024,
            string storageContainerName = "logs")
        {
            this.textFormatter = textFormatter;
            this.blobServiceClient = blobServiceClient;
            this.storageContainerName = storageContainerName;
            this.eventHubClient = eventHubClient;
            this.eventSizeLimit = eventSizeLimit;
        }

        public void Emit(LogEvent logEventPrm)
        {
            var blobUrl = string.Empty;
            var logEvent = logEventPrm;
            if (AuditEventPayload.DoesAuditEventPayloadExceedLimit(this.textFormatter, logEventPrm, this.eventSizeLimit))
            {
                blobUrl = this.UploadToBlob(logEventPrm);
                logEvent = AuditEventPayload.AuditEventMessageTransformation(logEventPrm, blobUrl);
            }

            this.PushToEventHub(logEvent);
        }

        /// <summary>
        /// Handles uploading to Azure blob
        /// </summary>
        /// <param name="logEvent">Log event</param>
        /// <returns>blob url</returns>
        private string UploadToBlob(LogEvent logEvent)
        {
            var content = AzureBlobServiceHelper.PrepareBlobContentForUpload(this.textFormatter, logEvent);
            var eventId = string.Empty;

            // Get Event Id value and use it as blob name
            LogEventPropertyValue eventIdValue;
            logEvent.Properties.TryGetValue("_EventId", out eventIdValue);
            if (eventIdValue != null)
            {
                eventId = eventIdValue.ToString();
            }

            var blobUrl = AzureBlobServiceProvider.UploadBlob(this.blobServiceClient, this.storageContainerName, eventId, content);
            return blobUrl;
        }

        /// <summary>
        /// Push message to Event hub
        /// </summary>
        /// <param name="logEvent">Log event</param>
        private void PushToEventHub(LogEvent logEvent)
        {
            var eventHubData = AzureEventHubServiceHelper.PrepareEventHubMessageContent(this.textFormatter, logEvent);
            this.eventHubClient.SendAsync(eventHubData, Guid.NewGuid().ToString()).GetAwaiter().GetResult();
        }
    }
}