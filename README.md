![Nuget](https://img.shields.io/nuget/v/:Web349.Logging)

# Web349 Logging Library
This .NET Core 6 library contains the logging functionality used throughout Web349's internal and external projects. Contains a powerful Datadog logger.

## ENV variable configuration
| Required | Name | Type | Default | Notes |
| - | - | - | - | - |
| | `WEB349_LOGGING_LOGLEVEL` | `string` | `Verbose` | The log level. Must be any of the following values: `Silent`, `Fatal`, `Error`, `Warning`, `Information`, `Debug`, `Verbose` |
| | `WEB349_LOGGING_CENSORSHIP_ENABLED` | `bool` | `true` | Toggles censorship of potentially sensitive information in the enriched fields of log events. The function looks for keywords configured in the `WEB349_LOGGING_CENSORSHIP_KEYWORDS` env var. |
| | `WEB349_LOGGING_CENSORSHIP_KEYWORDS` | `string` | `api;key;secret;credential;auth;cookie;login` | A semi-colon (;) delimited list of keywords to look for in the enriched fields of log events. Setting this ENV var to a custom value will replace the default values, unless explicitely specified. |
| &#9989; | `WEB349_LOGGING_DATADOG_APIKEY` | `string` | | Your Datadog API key |
| &#9989; | `WEB349_LOGGING_DATADOG_SITE` | `string` | | The Datadog site to send logs to. Must be any of the following values: `US1`, `US3`, `US5`, `EU`, `AP1`, `US1_GOV` |
| | `WEB349_LOGGING_DATADOG_SOURCE` | `string` | | Fills the Source field in Datadog |
| | `WEB349_LOGGING_DATADOG_SERVICE` | `string` | | Fills the Service field in Datadog |
| | `WEB349_LOGGING_DATADOG_HOST` | `string` | | Fills the Host field in Datadog |
| |  `WEB349_LOGGING_DATADOG_COMPRESSLOGS` | `bool` | `true` | Set to `true` to enable `gzip` compression |
| |  `WEB349_LOGGING_DATADOG_BATCH_SIZE` | `int` | `10` | The maximum size of a single log event batch. |
| |  `WEB349_LOGGING_DATADOG_BATCH_AGE` | `int` | `5` | The maximum age, in seconds, of a single log event batch that has not reached its maximum batch size |
| |  `WEB349_LOGGING_DATADOG_HTTPCLIENT_TIMEOUT` | `int` | `10` | Underlying `HttpClient` timeout in seconds |
| |  `WEB349_LOGGING_SLACK_HTTPCLIENT_TIMEOUT` | `int` | `10` | Underlying `HttpClient` timeout in seconds for calling the registered Webhook URL |
| |  `WEB349_LOGGING_SLACK_WEBHOOKURL_<NAME>` | `string` |  | A `SlackLogger` needs a name argument in its constructor that should match up with this ENV var |
| | `WEB349_LOGGING_DISPATCHER_DELAY_IDLE` | `int` | `1000` | The delay, in miliseconds, for the log batch dispatcher to wait in between polls while idling. |
| | `WEB349_LOGGING_DISPATCHER_DELAY_ACTIVE` | `int` | `100` | The delay, in miliseconds, for the log batch dispatcher to wait in between polls while processing log events. |

## Requirements
* .NET Core 6 [Download at dot.net](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* `System.Text.Json` for JSON serialization

## Notes
* Each instance of a `Logger` class creates its own context `logger.Context` for identification purposes.
* While technically possible, multiple threads should not share the same `Logger` instance.
* Each `Logger` context keeps track of an `int64` Event ID field.
* At the moment of writing `SlackLogger` only supports sending of unenriched, text-only messages.
* The `SlackLogger` name has the follow regular expression limitation: `^[a-zA-Z0-9_]+$`.
* The Dispatcher settings only apply to `Logger` implementations that use a producer/consumer dispatching service to send off the logs (e.g., `SlackLogger` and `DatadogLogger`).

## Datadog logger usage examples

### Logging regular information
```
Logger logger = new DatadogLogger();
logger.LogInformation("This is the message that will appear in Datadog");
```

### Logging enriched information
```
Logger logger = new DatadogLogger();
logger
    .Enrich("fieldName", 123)
    .Enrich("anotherFieldName", "some string value")
    .Enrich("foo", serializableBarObject)
.LogInformation("This is the message that will appear in Datadog, with the three fields enriched above");
```

### Logging an Exception
```
Logger logger = new DatadogLogger();
try
{
	throw new Exception("oops");
}
catch(Exception ex)
{
	logger.LogError(ex.Message, ex);
}
```

## Slack logger usage examples

### Logging regular information
```
// the Slack webhook URL will match to the value set to WEB349_LOGGING_SLACK_WEBHOOKURL_SERVICENAME
// dashes (-) will be replace with an underscore (_) and spaces are removed.
Logger serviceNameLogger = new SlackLogger("serviceName");
serviceNameLogger.LogInformation("a message");
```

## Information censorship usage example

The library has the ability to censor potentially sensitive information from enriched fields. See `WEB349_LOGGING_CENSORSHIP_KEYWORDS` for more information. The following is an example of information censorship:
```
Logger logger = new DatadogLogger();
logger
    .Enrich("myFieldNotCensored", obj)
    .Enrich("apiKeyBeingCensored", apiKeyStr) // this field contains 'api' and its value will be censored to '***'
.LogDebug("Hello friend, this is a DEBUG log event with one censored and one uncensored field");
```

## Contribution
Want to help? Comments? Love? Hate? Open up an Issueo or Pull Request on Github: https://github.com/web349/logging

## Disclaimer
This software is provided for free, AS IS, without any guarantees whatsoever, because I felt there were no easy-to-use Datadog loggers for .NET Core.
