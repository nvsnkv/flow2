# Flow - an expense tracking tools
__Flow__ is a set of command line tools designed to simplify expences tracking, analysis and forecast. Working in conjuction with your favorite editors for JSON and CSV files it helps to maintain your budget.

## How to use it?
### Prerequisites
__Flow__  works with a PostgreSQL database, so you'll need to spin one up and save connection string into flow configuration. Before the first use, you need to apply database migrations from `Flow.Infrastructure.Storage.Migrations` project using [EF Core .NET Core CLI tools](https://docs.microsoft.com/en-us/ef/core/get-started/overview/install#get-the-net-core-cli-tools). They are already referenced in `src\.config\dotnet-tools.json` so you'll just need to run `dotnet tool update` as mentioned in the guide.
Once tools are installed, please use `dotnet database update -- %CONNECTION_STRING_HERE%` command in _Migrations_ project folder. Don't forget to replace `%CONNECTION_STRING_HERE%` with actual connection string.

__Flow__ works best together with file editors, but they are optional. Apps can always record output to file or print it to STDOUT so you can examine the results of the command without any editor. 

### Configuration
TBD

### Expences tracking
TBD

### Flow analysis
TBD

### Forecast
TBD

### Other noteworthy things
TBD

## Development notes
TBD

## License
not yet defined

