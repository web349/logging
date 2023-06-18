# Web349 Logging Library
This .NET Core 6 library contains the logging functionality used throughout Web349's internal and external projects. Contains a powerful Datadog logger.

## ENV variable configuration
| Required | Name | Type | Default value | Description |
| - | - | - | - | - |
| &#9989; | `WEB349_LOGGING_DATADOG_APIKEY` | `string` | | Your Datadog API key |
| &#9989; | `WEB349_LOGGING_DATADOG_SITE` | `string` | | The Datadog site to send logs to. Must be any of the following values: `US1`, `US3`, `US5`, `EU`, `AP1`, `US1_GOV` |
| &#9989; | `WEB349_LOGGING_LOGLEVEL` | `string` | | The log level. Must be any of the following values: `Silent`, `Fatal`, `Error`, `Warning`, `Information`, `Debug`, `Verbose` |
|  | `WEB349_LOGGING_DATADOG_SOURCE` | `string` | | Fills the Source field in Datadog |
| | `WEB349_LOGGING_DATADOG_SERVICE` | `string` | | Fills the Service field in Datadog |
| | `WEB349_LOGGING_DATADOG_HOST` | `string` | | Fills the Host field in Datadog |
| |  `WEB349_LOGGING_DATADOG_COMPRESSLOGS` | `bool` | `true` | Set to `true` to enable `gzip` compression |
| |  `WEB349_LOGGING_DATADOG_BATCH_SIZE` | `int` | `10` | The maximum size of a single log event batch. |
| |  `WEB349_LOGGING_DATADOG_BATCH_AGE` | `int` | `5` | The maximum age, in seconds, of a single log event batch that has not reached its maximum batch size |
| | `WEB349_LOGGING_DISPATCHER_DELAY_IDLE` | `int` | `1000` | The delay, in miliseconds, for the log batch dispatcher to wait in between polls while idling |
| | `WEB349_LOGGING_DISPATCHER_DELAY_ACTIVE` | `int` | `10` | The delay, in miliseconds, for the log batch dispatcher to wait in between polls while processing log events |

## Requirements
* .NET Core 6
* System.Text.Json for JSON serialization

## Notes
* Each instance of a `Logger` class creates its own context `logger.Context` for identification purposes.
* While technically possible, multiple threads should not share the same `Logger` instance.
* Each `Logger` context keeps track of an `int64` Event ID field.

## Usage examples

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