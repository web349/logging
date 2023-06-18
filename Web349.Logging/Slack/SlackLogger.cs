using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web349.Logging.Enums;

namespace Web349.Logging.Slack
{
    public class SlackLogger : Logger
    {
        public SlackLogger(string name)
        {
            this.dispatcher = new SlackDispatcher(this.Context, name);
        }

        public SlackLogger(string context, string name) : base(context)
        {
            this.dispatcher = new SlackDispatcher(this.Context, name);
        }

        protected override void WriteLine(string message, LogLevel logLevel, Exception exception = null)
        {
            if (this.LogLevel < logLevel)
            {
                return;
            }

            // create a new log entry with basic data
            LogEntry logEntry = new(logLevel, this.GetEventId(), this.Context, message, exception);

            // dump enrichments into the log entry
            logEntry.Enrich(dynamicEnrichments);
            logEntry.Enrich(staticEnrichments);

            // enrich DD-specific
            //logEntry.Enrich("ddsource", this.Source);
            //logEntry.Enrich("service", this.Service);
            //logEntry.Enrich("hostname", this.Host);

            // serialize it to json
            string msg = logEntry.ToString();

            // and off ya go
            dispatcher.Enqueue(msg);

            // clear dynamic enrichments, resetting state for the next log event
            ClearDynamicEnrichments();
        }
    }
}
