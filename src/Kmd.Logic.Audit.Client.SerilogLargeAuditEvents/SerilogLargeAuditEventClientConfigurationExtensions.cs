using System;
using System.Collections.Generic;
using System.Text;
using Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink;
using Serilog;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public static class SerilogLargeAuditEventClientConfigurationExtensions
    {
        public static LoggerConfiguration CreateAzureBlobOrEventHubConfiguration(this SerilogLargeAuditEventClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var configBuilder = new LoggerConfiguration()
                    .Enrich.With(new EventIdEnricher())
                    .Enrich.With(new CreatedDateTimeEnricher())
                    .Enrich.WithProperty("_EventSource", config.EventSource)
                    .WriteTo.AzureBlobOrEventHub(
                        storageConnectionString: config.StorageConnectionString,
                        storageContainerName: config.StorageContainerName,
                        eventhubConnectionString: config.EventhubConnectionString,
                        eventHubName: config.AuditEventTopic,
                        eventSizeLimitInBytes: config.EventSizeLimitinBytes,
                        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter());

            if (config.EnrichFromLogContext == true)
            {
                configBuilder = configBuilder.Enrich.FromLogContext();
            }

            return configBuilder;
        }
    }
}
