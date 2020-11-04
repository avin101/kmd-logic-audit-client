using System;
using System.Collections.Generic;
using System.Text;

namespace AuditClient
{
    class ProgramConfigIngestion
    {
        public int NumberOfEventsToSend { get; set; } = 1;

        public int NumberOfThreads { get; set; } = 1;

        public string ConnectionString { get; set; }

        public string AuditEventTopic { get; set; } = "auditcustomsink";

        public string EventSource { get; set; }

        public string BlobConnectionString { get; set; } = "DefaultEndpointsProtocol=https;AccountName=kmdaisuyw;AccountKey=GqeuOfyImMYP4zUdgiXQi1h62Mvaq4NnqilRG1aFw+YEqPN2lw0YQQMX0rGmLOztnuc8NOGk8in89ZFQ0DBH1g==;EndpointSuffix=core.windows.net";

        public string BlobAccountName { get; set; } = "kmdaisuyw";

        public string BlobContainerName { get; set; } = "large-audit-events1";

        public int EventSizeLimitinBytes { get; set; }
    }
}
