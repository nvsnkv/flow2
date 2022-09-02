# Flow - an expense tracking tools
![Build and test](https://github.com/nvsnkv/flow2/actions/workflows/dotnet.yml/badge.svg?branch=master)

__Flow__ is a set of command line tools designed to simplify expences tracking, analysis and forecast. Working in conjuction with your favorite editors for JSON and CSV files it helps to maintain your budget.

# How to use it?
## Prerequisites
__Flow__  works with a PostgreSQL database, so you'll need to spin one up and save connection string into flow configuration. You can use `src\docker-compose.yaml` file to spin up a new database or use an existing instance if you have one.

Before the first use, you need to apply database migrations from `Flow.Infrastructure.Storage.Migrations` project using [EF Core .NET Core CLI tools](https://docs.microsoft.com/en-us/ef/core/get-started/overview/install#get-the-net-core-cli-tools). They are already referenced in `src\.config\dotnet-tools.json` so you'll just need to run `dotnet tool update` as mentioned in the guide.
Once tools are installed, please use `dotnet ef database update -- %CONNECTION_STRING_HERE%` command in _Migrations_ project folder. Don't forget to replace `%CONNECTION_STRING_HERE%` with actual connection string.

__Flow__ works best together with file editors, but they are optional. Apps can always record output to file or print it to STDOUT so you can examine the results of the command without any editor.

## Installation
1. Clone repository
2. Navigate to `src` folder
3. Build solution using `dotnet build` or Visual Studio
4. Navigate to `src\l3\CommandLine\Flow.Hosts.EntryPoint.Cli` folder
5. Publish bundle to preferrable ouptut directory (ex. `dotnet publish -c Release -o ..\..\..\..\build\cli-bundle`)
6. Navigate to target folder with the bundle
7. run `flow bundle register` to add folder with bundle to path and set appsettings.json as a global config file for all flow apps

Now you can use all CLI tools in any folder.

## Configuration
CLI tools uses [.NET Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration) with [json](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#json-configuration-provider) and [Environment Variables](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#environment-variable-configuration-provider) providers available. 
You can write settings to _appsettings.json_ file in the working directory, specify a separate file location in `FLOW_CONFIG_FILE` environment variable or set environment variables to override anyting what's already in settings file.

`flow bundle configure` shorthand allows to open editor for configuration file from `FLOW_CONFIG_FILE`  environment variable.

There are quite a few settings to provide, and most of them optional:
* `flow:ConnectionString` - required. A connection string to PostgreSQL database that handles data;
* `flow:CultureCode` - optional. Culture code (2-letter language, 2-letter region) that should be used to read or write data. App will use current culture by default;
* `flow:Editor:JSON` - a path to external editor for JSON files. Empty by default;
* `flow:Editor:CSV` - a path to external editor for CSV files. Empty by default;

## Running tools
Once registered and configured, CLI bundle is ready to work. You can use it on  your own or add tools into batch scripts as any other regular CLI application.
All tools have `help command` that briefly explains supported commands and its options. More detailed guides can be found below:
* [Expenses Tracking](docs/expenses_tracking.md)
* [Flow Analysis](docs/flow_analysis.md)


# Development notes
Please refer to [Architecture Decision Records](docs/adr/)

# License
MIT
 
