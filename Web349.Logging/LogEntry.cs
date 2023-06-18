using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Web349.Logging.Enums;

namespace Web349.Logging
{
    public class LogEntry
    {
        private JsonObject obj = new();

        public string Message { get; set; }
        public string Context { get; set; }
        public long EventId { get; set; }
        public Exception Exception { get; set; }
        public LogLevel LogLevel { get; set; }

        public LogEntry(LogLevel logLevel, long eventId, string context, string message, Exception exception = null)
        {
            EventId = eventId;
            LogLevel = logLevel;
            Context = context;
            Message = message;
            Exception = exception;
        }

        public LogEntry Enrich(string name, object value)
        {
            return AddEnrichment(name, value);
        }

        public LogEntry Enrich(Dictionary<string, object> enrichments)
        {
            foreach (var enrichment in enrichments)
            {
                AddEnrichment(enrichment.Key, enrichment.Value);
            }

            return this;
        }

        public LogEntry AddEnrichment(string name, object value)
        {
            obj.TryAdd(SanitizeJsonFieldName(name), JsonValue.Create(value));
            return this;
        }

        public LogEntry RemoveEnrichment(string name)
        {
            obj.Remove(SanitizeJsonFieldName(name));
            return this;
        }

        public LogEntry ClearEnrichments()
        {
            obj.Clear();
            return this;
        }

        public override string ToString()
        {
            obj["context"] = JsonValue.Create(this.Context);
            obj["message"] = JsonValue.Create(this.Message);
            obj["eventID"] = JsonValue.Create(this.EventId);
            obj["level"] = JsonValue.Create(this.LogLevel.ToString().ToLower());

            if (this.Exception != null)
            {
                JsonObject ex = new();

                ex["message"] = this.Exception.Message;
                ex["stackTrace"] = this.Exception.StackTrace;
                ex["source"] = this.Exception.Source;

                if (this.Exception.InnerException != null)
                {
                    JsonObject inner = new();
                    inner["message"] = this.Exception.InnerException.Message;
                    inner["stackTrace"] = this.Exception.InnerException.StackTrace;
                    inner["source"] = this.Exception.InnerException.Source;
                    ex["innerException"] = inner;
                }

                obj["exception"] = ex;
            }

            return obj.ToJsonString();
        }

        private string SanitizeJsonFieldName(string name)
        {
            return name.Replace(" ", "_").Trim();
        }
    }
}
