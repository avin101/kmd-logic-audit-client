﻿using System;
using Serilog.Core;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public class SerilogLargeAuditEventClient : IAudit, IDisposable
    {
        private readonly Logger logger;
        private readonly bool disposeLogger;
        private readonly SerilogLoggerAudit audit;

        public SerilogLargeAuditEventClient(SerilogLargeAuditEventClientConfiguration config)
        {
            this.logger = config.CreateDefaultConfiguration().CreateLogger();
            this.disposeLogger = true;
            this.audit = new SerilogLoggerAudit(this.logger);
        }

        private SerilogLargeAuditEventClient(Logger logger, bool disposeLogger)
        {
            this.logger = logger;
            this.disposeLogger = disposeLogger;
            this.audit = new SerilogLoggerAudit(this.logger);
        }

        public static SerilogLargeAuditEventClient CreateCustomized(Logger logger, bool disposeLogger = true)
        {
            return new SerilogLargeAuditEventClient(logger, disposeLogger);
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            this.audit.Write(messageTemplate, propertyValues);
        }

        public IAudit ForContext(string propertyName, object value, bool captureObjectStructure = false)
        {
            return this.audit.ForContext(propertyName, value, captureObjectStructure);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.disposeLogger)
            {
                this.logger.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
