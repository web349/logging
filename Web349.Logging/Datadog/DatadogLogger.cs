using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web349.Logging.Enums;

namespace Web349.Logging.Datadog
{
    public class DatadogLogger : Logger
    {
        public string Source { get; set; }
        public string Service { get; set; }
        public string Host { get; set; }

        public DatadogLogger() : this(null)
        {
        }

        public DatadogLogger(string context) : base(context)
        {
            this.dispatcher = new DatadogDispatcher(this.Context);
            this.Source = Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_SOURCE") ?? this.Context;
            this.Service = Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_SERVICE");
            this.Host = Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_HOST");
        }

        protected override string WriteLine(string message, LogLevel logLevel, Exception exception = null)
        {
            if (this.LogLevel < logLevel)
            {
                return null;
            }

            // create a new log entry with basic data
            LogEntry logEntry = new(logLevel, this.GetEventId(), this.Context, message, exception);

            // dump enrichments into the log entry
            logEntry.Enrich(dynamicEnrichments);
            logEntry.Enrich(staticEnrichments);

            // enrich DD-specific
            logEntry.Enrich("ddsource", this.Source);
            logEntry.Enrich("service", this.Service);
            logEntry.Enrich("hostname", this.Host);

            // serialize it to json
            string msg = logEntry.ToString();

            // and off ya go
            dispatcher.Enqueue(msg);

            // clear dynamic enrichments, resetting state for the next log event
            ClearDynamicEnrichments();

            return msg;
        }
    }
}
