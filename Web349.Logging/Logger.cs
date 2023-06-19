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
        private string[] censorshipKeywords;

        private long eventId = 0;
        protected readonly Dictionary<string, object> dynamicEnrichments = new();
        protected readonly Dictionary<string, object> staticEnrichments = new();
        protected Dispatcher dispatcher = null;

        public LogLevel LogLevel { get; set; } = LogLevel.Verbose;
        public string Context { get; private set; }

        public bool IsCensorshipEnabled { get; set; }

        public Logger() : this(null)
        {
        }

        public Logger(string context)
        {
            this.Context = context?.ToLower()?.Trim() ?? Guid.NewGuid().ToString().ToLower();
            if (!Enum.TryParse<LogLevel>(Environment.GetEnvironmentVariable("WEB349_LOGGING_LOGLEVEL"), true, out LogLevel level))
            {
                throw new Exception($"Unable to determine default log level. Please configure WEB349_LOGGING_LOGLEVEL to be one of following values: Silent, Fatal, Error, Warning, Information, Debug, Verbose");
            }
            this.LogLevel = level;
            this.IsCensorshipEnabled = Convert.ToBoolean(Environment.GetEnvironmentVariable("WEB349_LOGGING_CENSORSHIP_ENABLED") ?? "true");

            string censorshipKeywordsEnv = Environment.GetEnvironmentVariable("WEB349_LOGGING_CENSORSHIP_KEYWORDS") ?? "api;key;secret;credential;auth;cookie;login";
            if (!string.IsNullOrEmpty(censorshipKeywordsEnv))
            {
                this.censorshipKeywords = censorshipKeywordsEnv.Trim().ToLower().Split(';');
            }
        }

        public Logger Enrich(string name, object value, bool isStatic = false)
        {
            if (IsCensorshipEnabled && IsSensitiveInformation(name))
            {
                value = "***";
            }

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

        public string LogFatal(string message)
        {
            return WriteLine(message, LogLevel.Fatal);
        }

        public string LogFatal(string message, Exception exception)
        {
            return WriteLine(message, LogLevel.Fatal, exception);
        }

        public string LogError(string message)
        {
            return WriteLine(message, LogLevel.Error);
        }

        public string LogError(string message, Exception exception)
        {
            return WriteLine(message, LogLevel.Error, exception);
        }

        public string LogWarning(string message)
        {
            return WriteLine(message, LogLevel.Warning);
        }

        public string LogInformation(string message)
        {
            return WriteLine(message, LogLevel.Information);
        }

        public string LogDebug(string message)
        {
            return WriteLine(message, LogLevel.Debug);
        }

        public string LogVerbose(string message)
        {
            return WriteLine(message, LogLevel.Verbose);
        }

        protected long GetEventId()
        {
            return Interlocked.Increment(ref eventId);
        }

        protected bool IsSensitiveInformation(string name)
        {
            // try to determine if the supplied name/key could possibly contain sensitive information, such as api keys or credentials
            string lname = name.ToLower().Trim();
            bool hasKeyword = censorshipKeywords.Where(x => lname.Contains(x, StringComparison.OrdinalIgnoreCase)).Count() != 0;
            return hasKeyword;
        }

        protected abstract string WriteLine(string message, LogLevel logLevel, Exception exception = null);
    }
}
