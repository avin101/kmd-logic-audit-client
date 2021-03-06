﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Kmd.Logic.Audit.Client.Sample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)

                // Without *some* values in the config, we get a null instance of `ProgramConfig`
                // however if we ensure at least one value is in the config collection, then we get
                // an instance, no matter what the key/value is.
                .AddInMemoryCollection(initialData: new Dictionary<string, string> { { "dummy", "value" }, })
                .Build()
                .Get<ProgramConfig>();

            if (string.IsNullOrEmpty(config?.Ingestion?.ConnectionString))
            {
                Console.WriteLine("You need to specify a connection string (e.g. --Ingestion:ConnectionString=\"test\")");
                return;
            }

            var clientConfig = new SerilogAzureEventHubs.SerilogAzureEventHubsAuditClientConfiguration
                {
                    EventSource = config.Ingestion.EventSource ?? $"{typeof(Program).Assembly.GetName().Name} on {Environment.MachineName}",
                    AuditEventTopic = config.Ingestion.AuditEventTopic,
                    ConnectionString = config.Ingestion.ConnectionString,
                    EnrichFromLogContext = config.Client.EnrichFromLogContext,
                };

            using (var client = new SerilogAzureEventHubs.SerilogAzureEventHubsAuditClient(clientConfig))
            {
                var audit = (IAudit)client;
                var name = typeof(Program).Assembly.GetName().Name;
                var version = typeof(Program).Assembly.GetName().Version;

                Console.WriteLine(
                    "Sending {0} ({3} threads) audit events to {1} at {2}",
                    config.Ingestion.NumberOfEventsToSend,
                    clientConfig.AuditEventTopic,
                    clientConfig.ConnectionString,
                    config.Ingestion.NumberOfThreads);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var groupId = Guid.NewGuid();

                if (clientConfig.EnrichFromLogContext == true)
                {
                    Console.WriteLine($"The GroupId {groupId} will be attached to each event");
                }

                using (Serilog.Context.LogContext.PushProperty("GroupId", groupId))
                {
                    Enumerable
                        .Range(0, config.Ingestion.NumberOfEventsToSend)
                        .AsParallel()
                        .WithDegreeOfParallelism(config.Ingestion.NumberOfThreads)
                        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                        .Select(i =>
                        {
                            audit.Write("Hello number {IterationNum} from {Application} v{Version}", i, name, version);

                            var numDividedBy10Or1 = config.Ingestion.NumberOfEventsToSend < 10
                                ? 1
                                : config.Ingestion.NumberOfEventsToSend / 10;

                            if ((i % numDividedBy10Or1) == 0)
                            {
                                Console.WriteLine($"Sent {i} events so far...");
                            }
                            return i;
                        })
                        .ToArray();
                }

                Console.WriteLine("Finished in {0}ms", sw.ElapsedMilliseconds);
            }
        }
    }
}
