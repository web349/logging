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

        protected override string WriteLine(string message, LogLevel logLevel, Exception exception = null)
        {
            if (this.LogLevel < logLevel)
            {
                return null;
            }

            long eventId = this.GetEventId();

            // create a new log entry with basic data
            LogEntry logEntry = new(logLevel, eventId, this.Context, message, exception);

            // dump enrichments into the log entry
            logEntry.Enrich(dynamicEnrichments);
            logEntry.Enrich(staticEnrichments);

            // need to add enrichments to the message. but how to format properly? I'm not a designer :-)
            string msg = $"[{logLevel.ToString().ToUpper()}] [{eventId}] [{DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff")}] - {message}";

            // and off ya go
            dispatcher.Enqueue(msg);

            // clear dynamic enrichments, resetting state for the next log event
            ClearDynamicEnrichments();

            return msg;
        }
    }
}
