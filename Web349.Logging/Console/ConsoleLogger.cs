using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web349.Logging.Enums;

namespace Web349.Logging.Console
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger() : this(null)
        {
        }

        public ConsoleLogger(string context) : base(context)
        {
        }

        protected override string WriteLine(string message, LogLevel logLevel, Exception exception = null)
        {
            switch (logLevel)
            {
                case LogLevel.Fatal:
                    System.Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case LogLevel.Error:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Information:
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.Debug:
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Verbose:
                    System.Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    System.Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            long eventId = this.GetEventId();
            string msg = $"[{logLevel.ToString().ToUpper()}] [{eventId}] [{DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff")}] - {message}";
            
            System.Console.WriteLine(msg);

            if (exception != null)
            {
                string exceptionMsg = $"[{logLevel.ToString().ToUpper()}] [{eventId}] [{DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff")}] - {exception.Message}";
                msg += $"\n{exceptionMsg}";

                System.Console.WriteLine(exceptionMsg);

                if (exception.InnerException != null)
                {
                    string innerExceptionMsg = $"[{logLevel.ToString().ToUpper()}] [{eventId}] [{DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff")}] - {exception.InnerException.Message}";
                    msg += $"\n{innerExceptionMsg}";
                    System.Console.WriteLine(innerExceptionMsg);
                }
            }

            System.Console.ForegroundColor = ConsoleColor.Gray;

            return msg;
        }
    }
}
