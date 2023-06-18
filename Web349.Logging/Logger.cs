using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web349.Logging.Enums;

namespace Web349.Logging
{
    public abstract class Logger
    {
        private long eventId = 0;
        protected Dictionary<string, object> dynamicEnrichments = new();
        protected Dictionary<string, object> staticEnrichments = new();
        protected Dispatcher dispatcher = null;

        public LogLevel LogLevel { get; set; } = LogLevel.Verbose;
        public string Context { get; private set; }

        public Logger()
        {
            this.Context = Guid.NewGuid().ToString().ToLower();
            if (!Enum.TryParse<LogLevel>(Environment.GetEnvironmentVariable("WEB349_LOGGING_LOGLEVEL"), true, out LogLevel level))
            {
                throw new Exception($"Unable to determine default log level. Please configure WEB349_LOGGING_LOGLEVEL to be one of following values: Silent, Fatal, Error, Warning, Information, Debug, Verbose");
            }
        }

        public Logger(string context)
        {
            this.Context = context.ToLower().Trim();
        }

        public Logger Enrich(string name, object value, bool isStatic = false)
        {
            if (!isStatic)
            {
                dynamicEnrichments.TryAdd(name, value);
            }
            else
            {
                staticEnrichments.TryAdd(name, value);
            }

            return this;
        }

        public void ClearDynamicEnrichments()
        {
            staticEnrichments.Clear();
        }

        public void ClearStaticEnrichments()
        {
            staticEnrichments.Clear();
        }

        public void LogFatal(string message)
        {
            WriteLine(message, LogLevel.Fatal);
        }

        public void LogFatal(string message, Exception exception)
        {
            WriteLine(message, LogLevel.Fatal, exception);
        }

        public void LogError(string message)
        {
            WriteLine(message, LogLevel.Error);
        }

        public void LogError(string message, Exception exception)
        {
            WriteLine(message, LogLevel.Error, exception);
        }

        public void LogWarning(string message)
        {
            WriteLine(message, LogLevel.Warning);
        }

        public void LogInformation(string message)
        {
            WriteLine(message, LogLevel.Information);
        }

        public void LogDebug(string message)
        {
            WriteLine(message, LogLevel.Debug);
        }

        public void LogVerbose(string message)
        {
            WriteLine(message, LogLevel.Verbose);
        }

        protected long GetEventId()
        {
            return Interlocked.Increment(ref eventId);
        }

        protected abstract void WriteLine(string message, LogLevel logLevel, Exception exception = null);
    }
}
