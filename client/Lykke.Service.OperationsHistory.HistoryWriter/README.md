# Lykke.Service.OperationsHistory.HistoryWriter

This history writer is intended to push messages provided to history queue.

## Getting Started

Just install the package from nuget:
```
Install-Package Lykke.Service.OperationsHistory.HistoryWriter
```

### Prerequisites

In order for history writer to work properly it needs Azure account connection string to be provided during the instance initialization.

## Usage example
```csharp
// create the instance of the writer
var azureConnectionString = "<actual value>";
var writer = new HistoryWriter(azureConnectionString);

// the writer can be registered within IoC container for further usage using IHistoryWriter interface

// create and initialize new message
var message = new HistoryEntry();

// push the message to the queue
await writer.Push(message);
```

## Built With

* [Newtonsoft.Json](http://www.newtonsoft.com/json/) - The JSON framework for .NET

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the tags on this repository.

## Authors

* [Lykke](https://www.lykke.com)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
