using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web349.Logging
{
    public abstract class Dispatcher : IDisposable
    {
        public string Context { get; set; }
        public int DelayIdle => Convert.ToInt32(Environment.GetEnvironmentVariable("WEB349_LOGGING_DISPATCHER_DELAY_IDLE") ?? "1000");
        public int DelayActive => Convert.ToInt32(Environment.GetEnvironmentVariable("WEB349_LOGGING_DISPATCHER_DELAY_ACTIVE") ?? "100");

        protected Dispatcher(string context)
        {
            this.Context = context.Trim().ToLower();
        }

        public abstract void Enqueue(string msg);
        public abstract void Dispose();
        public abstract Task Run();
    }
}
