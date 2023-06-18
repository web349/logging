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
        public ConsoleLogger()
        {
        }

        public ConsoleLogger(string context) : base(context)
        {
        }

        protected override void WriteLine(string message, LogLevel logLevel, Exception exception = null)
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

            System.Console.WriteLine($"[{logLevel.ToString().ToUpper()}] [{eventId}] [{DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff")}] - {message}");

            if (exception != null)
            {
                System.Console.WriteLine($"[{logLevel.ToString().ToUpper()}] [{eventId}] [{DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff")}] - {exception.Message}");

                if (exception.InnerException != null)
                {
                    System.Console.WriteLine($"[{logLevel.ToString().ToUpper()}] [{eventId}] [{DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff")}] - {exception.InnerException.Message}");
                }
            }

            System.Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
