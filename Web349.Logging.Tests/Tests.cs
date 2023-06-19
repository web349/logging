using Web349.Logging.Datadog;
using Web349.Logging.Enums;

namespace Web349.Logging.Tests
{
    public class Tests
    {
        private Logger logger;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("WEB349_LOGGING_LOGLEVEL", "Verbose");

            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_SOURCE", "Web349.Logging.Tests");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_SERVICE", "visualstudio2022");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_SITE", "EU");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_HOST", "localhost");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_APIKEY", "<API KEY HERE>");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_COMPRESSLOGS", "true");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_BATCH_SIZE", "2");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_BATCH_AGE", "30");

            logger = new DatadogLogger("testcontext");
        }

        [Test]
        public void LogInformation()
        {
            string output = logger
                .Enrich("numberField", 123)
                .Enrich("stringField", "456")
                .Enrich("booleanField", true)
            .LogInformation("This is an INFO log message including 3 enrichments");

            Assert.AreEqual(@"{""numberField"":123,""stringField"":""456"",""booleanField"":true,""ddsource"":""Web349.Logging.Tests"",""service"":""visualstudio2022"",""hostname"":""localhost"",""context"":""testcontext"",""message"":""This is an INFO log message including 3 enrichments"",""eventID"":1,""level"":""information""}", output);
        }

        [Test]
        public void LogDebug()
        {
            string output = logger
                .Enrich("numberField", 123)
                .Enrich("stringField", "456")
                .Enrich("booleanField", true)
            .LogDebug("This is an DEBUG log message including 3 enrichments");

            Assert.AreEqual(@"{""numberField"":123,""stringField"":""456"",""booleanField"":true,""ddsource"":""Web349.Logging.Tests"",""service"":""visualstudio2022"",""hostname"":""localhost"",""context"":""testcontext"",""message"":""This is an DEBUG log message including 3 enrichments"",""eventID"":1,""level"":""debug""}", output);
        }

        [Test]
        public void LogError()
        {
            string output = logger
                .Enrich("numberField", 123)
                .Enrich("stringField", "456")
                .Enrich("booleanField", true)
            .LogError("This is an ERROR log message including 3 enrichments", new Exception("This is an exception object"));

            Assert.AreEqual(@"{""numberField"":123,""stringField"":""456"",""booleanField"":true,""ddsource"":""Web349.Logging.Tests"",""service"":""visualstudio2022"",""hostname"":""localhost"",""context"":""testcontext"",""message"":""This is an ERROR log message including 3 enrichments"",""eventID"":1,""level"":""error"",""exception"":{""message"":""This is an exception object"",""stackTrace"":null,""source"":null}}", output);
        }

        [Test]
        public void LogCensoredInformation()
        {
            string output = logger
                .Enrich("uncensoredInt32", 123)
                .Enrich("censoredApiKey", "my_secret_api_key_please_dont_tell")
            .LogError("This is an ERROR log message including 3 enrichments", new Exception("This is an exception object"));

            bool containsCensoredInformation = output.Contains(@"""censoredApiKey"":""***""");
            Assert.AreEqual(true, containsCensoredInformation);
        }

        [Test]
        public void GetDatadogSiteByNameEU()
        {
            string value = DatadogSite.GetByString("eu");
            Assert.AreEqual("https://http-intake.logs.datadoghq.eu/api/v2/logs", value);
        }
    }
}