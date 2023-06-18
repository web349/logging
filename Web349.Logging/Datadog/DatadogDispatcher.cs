using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Web349.Logging.Enums;

namespace Web349.Logging.Datadog
{
    public class DatadogDispatcher : Dispatcher
    {
        private readonly ConcurrentQueue<string> queue = new();
        private readonly HttpClient httpClient = new();
        private readonly Task task = null;
        private readonly string intakeUrl = null;
        private readonly string apiKey = null;
        private readonly int batchSize = 10;
        private readonly int batchAge = 5;

        public DatadogDispatcher(string context) : base(context)
        {
            this.intakeUrl = DatadogSite.GetByString(Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_SITE"));
            if (string.IsNullOrEmpty(this.intakeUrl))
            {
                throw new Exception("Datadog intake URL is empty. Configure WEB349_LOGGING_DATADOG_SITE to one of the following values: US1, US3, US5, EU, AP1, US1_GOV.");
            }

            if (!Uri.IsWellFormedUriString(this.intakeUrl, UriKind.Absolute))
            {
                throw new Exception($"Datadog intake URL is malformed: {this.intakeUrl}");
            }

            this.apiKey = Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_APIKEY");
            if (string.IsNullOrEmpty(this.apiKey))
            {
                throw new Exception("Datadog API key is empty. Configure WEB349_LOGGING_DATADOG_APIKEY to your API key.");
            }

            this.batchSize = Convert.ToInt32(Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_BATCH_SIZE") ?? "10");
            this.batchAge = Convert.ToInt32(Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_BATCH_AGE") ?? "5");

            this.httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_HTTPCLIENT_TIMEOUT") ?? "10"));

            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("DD-API-KEY", this.apiKey);

            string compressLogs = Environment.GetEnvironmentVariable("WEB349_LOGGING_DATADOG_COMPRESSLOGS") ?? "true";
            if (compressLogs.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Encoding", "gzip");
            }

            this.task = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

        public override void Enqueue(string msg)
        {
            queue.Enqueue(msg);
        }

        public override async Task Run()
        {
            DateTimeOffset start = DateTimeOffset.UtcNow;
            List<string> batch = new();

            while (true)
            {
                try
                {
                    // while the batch count is less than the maximum batch size (batchSize) OR the batch age is less than the maximum batch age (batchAge) AND an item got dequeue, add to the batch
                    while ((batch.Count < batchSize || (DateTimeOffset.UtcNow - start) < TimeSpan.FromSeconds(batchAge)) && queue.TryDequeue(out string msg))
                    {
                        batch.Add(msg);
                        await Task.Delay(DelayActive);
                    }

                    if (batch.Count != 0)
                    {
                        System.Console.WriteLine($"DatadogDispatcher({this.Context}) POST {batch.Count} log entries to {this.intakeUrl}");

                        HttpResponseMessage res = await this.httpClient.PostAsJsonAsync<List<string>>(this.intakeUrl, batch);
                        if (!res.IsSuccessStatusCode)
                        {
                            System.Console.ForegroundColor = System.ConsoleColor.Red;
                            System.Console.WriteLine($"DatadogDispatcher({this.Context}) http exception: failed to POST log batch to Datadog intake URL {this.intakeUrl} with status {res.StatusCode} ({res.ReasonPhrase})");
                            System.Console.ForegroundColor = System.ConsoleColor.Gray;
                        }

                        batch.Clear();
                        start = DateTimeOffset.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                    System.Console.WriteLine($"DatadogDispatcher({this.Context}) unhandled exception: {ex.Message}");
                    System.Console.ForegroundColor = System.ConsoleColor.Gray;
                }
                finally
                {
                    await Task.Delay(this.DelayIdle);

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
