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

        public string AuditEventTopic { get; set; } = "audit";

        public string EventSource { get; set; }

        public string BlobConnectionString { get; set; }

        public string BlobAccountName { get; set; }

        public string BlobContainerName { get; set; }

        public int EventSizeLimitinBytes { get; set; }
    }
}
