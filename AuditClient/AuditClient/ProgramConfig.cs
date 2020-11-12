using System;
using System.Collections.Generic;
using System.Text;

namespace AuditClient
{
    public class ProgramConfig
    {
        public ProgramConfigIngestion Ingestion { get; set; } = new ProgramConfigIngestion();

        public ProgramConfigClient Client { get; set; } = new ProgramConfigClient();
    }
}
