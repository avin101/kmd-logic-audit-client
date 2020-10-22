using System;
using Serilog.Core;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public class SerilogLargeAuditEventClient : IAudit, IDisposable
    {
        private readonly Logger customSinkLogger;
        private readonly bool disposeLogger;
        private readonly SerilogCustomSinkLoggerAudit auditToCustomSink;

        public SerilogLargeAuditEventClient(SerilogLargeAuditEventClientConfiguration config)
        {
            this.customSinkLogger = config.CreateAzureBlobOrEventHubConfiguration().CreateLogger();
            this.disposeLogger = true;
            this.auditToCustomSink = new SerilogCustomSinkLoggerAudit(this.customSinkLogger);
        }

        private SerilogLargeAuditEventClient(Logger logger, bool disposeLogger)
        {
            this.customSinkLogger = logger;
            this.disposeLogger = disposeLogger;
            this.auditToCustomSink = new SerilogCustomSinkLoggerAudit(this.customSinkLogger);
        }

        public static SerilogLargeAuditEventClient CreateCustomized(Logger logger, bool disposeLogger = true)
        {
            return new SerilogLargeAuditEventClient(logger, disposeLogger);
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            this.auditToCustomSink.Write(messageTemplate, propertyValues);
        }

        public IAudit ForContext(string propertyName, object value, bool captureObjectStructure = false)
        {
            return this.auditToCustomSink.ForContext(propertyName, value, captureObjectStructure);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.disposeLogger)
            {
                this.customSinkLogger.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
