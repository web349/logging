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
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_SOURCE", "Web349.Logging.Tests");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_SERVICE", "visualstudio2022");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_SITE", "EU");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_HOST", "localhost");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_APIKEY", "<API KEY HERE>");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_COMPRESSLOGS", "true");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_BATCH_SIZE", "2");
            Environment.SetEnvironmentVariable("WEB349_LOGGING_DATADOG_BATCH_AGE", "30");

            logger = new DatadogLogger();
        }

        [Test]
        public void LogInformation()
        {
            logger
                .Enrich("numberField", 123)
                .Enrich("stringField", "456")
                .Enrich("booleanField", true)
            .LogInformation("This is an INFO log message including 3 enrichments");

            Assert.Pass();
        }

        [Test]
        public void LogDebug()
        {
            logger
                .Enrich("numberField", 123)
                .Enrich("stringField", "456")
                .Enrich("booleanField", true)
            .LogDebug("This is an DEBUG log message including 3 enrichments");

            Assert.Pass();
        }

        [Test]
        public void LogError()
        {
            logger
                .Enrich("numberField", 123)
                .Enrich("stringField", "456")
                .Enrich("booleanField", true)
            .LogError("This is an ERROR log message including 3 enrichments", new Exception("This is an exception object"));

            Assert.Pass();
        }

        [Test]
        public void GetDatadogSiteByNameEU()
        {
            string value = DatadogSite.GetByString("eu");
            Assert.AreEqual("https://http-intake.logs.datadoghq.eu/api/v2/logs", value);
        }
    }
}