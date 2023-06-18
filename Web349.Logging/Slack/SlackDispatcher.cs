using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Web349.Logging.Enums;

namespace Web349.Logging.Slack
{
    public class SlackDispatcher : Dispatcher
    {
        private readonly ConcurrentQueue<string> queue = new();
        private readonly HttpClient httpClient = new();
        private readonly Task task = null;
        private readonly string webhookUrl = null;

        public SlackDispatcher(string context, string webhookUrlName) : base(context)
        {
            webhookUrlName = webhookUrlName.Trim().ToUpper();
            if (string.IsNullOrEmpty(webhookUrlName))
            {
                throw new Exception($"No Slack Webook URL name provided. Configure WEB349_LOGGING_SLACK_WEBHOOKURL_<NAME OF THE SLACK LOGGER>.");
            }

            string webhookUrlEnvVar = $"WEB349_LOGGING_SLACK_WEBHOOKURL_{webhookUrlName}";
            this.webhookUrl = Environment.GetEnvironmentVariable(webhookUrlName);

            if (string.IsNullOrEmpty(this.webhookUrl))
            {
                throw new Exception($"Slack webhook URL is empty. Configure WEB349_LOGGING_SLACK_WEBHOOKURL_{webhookUrlName} to contain a valid Slack webhook URL.");
            }

            if (!Uri.IsWellFormedUriString(this.webhookUrl, UriKind.Absolute))
            {
                throw new Exception($"Datadog intake URL is malformed: {this.webhookUrl}");
            }

            this.httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(Environment.GetEnvironmentVariable("WEB349_LOGGING_SLACK_HTTPCLIENT_TIMEOUT") ?? "10"));

            this.task = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

        public override void Enqueue(string msg)
        {
            queue.Enqueue(msg);
        }

        public override async Task Run()
        {
            while (true)
            {
                try
                {
                    while (queue.TryDequeue(out string msg))
                    {
                        SlackMessage message = new(msg);

                        HttpResponseMessage res = await httpClient.PostAsJsonAsync<SlackMessage>(webhookUrl, message);
                        if (!res.IsSuccessStatusCode)
                        {
                            System.Console.ForegroundColor = System.ConsoleColor.Red;
                            System.Console.WriteLine($"SlackDispatcher({Context}) http exception: failed to POST log batch to Slack webhook URL {webhookUrl} with status {res.StatusCode} ({res.ReasonPhrase})");
                            System.Console.ForegroundColor = System.ConsoleColor.Gray;
                        }

                        await Task.Delay(DelayActive);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                    System.Console.WriteLine($"SlackDispatcher({Context}) unhandled exception: {ex.Message}");
                    System.Console.ForegroundColor = System.ConsoleColor.Gray;
                }
                finally
                {
                    await Task.Delay(DelayIdle);
                }
            }
        }

        private bool disposed = false;
        public override void Dispose()
        {
            if (!disposed)
            {
                if (task != null)
                {
                    task.Dispose();
                }

                if (httpClient != null)
                {
                    httpClient.Dispose();
                }

                disposed = true;
            }
        }
    }
}
