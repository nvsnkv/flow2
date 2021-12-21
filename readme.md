# Flow - an expense tracking tools
__Flow__ is a set of command line tools designed to simplify expences tracking, analysis and forecast. Working in conjuction with your favorite editors for JSON and CSV files it helps to maintain your budget.

# How to use it?
## Prerequisites
__Flow__  works with a PostgreSQL database, so you'll need to spin one up and save connection string into flow configuration. Before the first use, you need to apply database migrations from `Flow.Infrastructure.Storage.Migrations` project using [EF Core .NET Core CLI tools](https://docs.microsoft.com/en-us/ef/core/get-started/overview/install#get-the-net-core-cli-tools). They are already referenced in `src\.config\dotnet-tools.json` so you'll just need to run `dotnet tool update` as mentioned in the guide.
Once tools are installed, please use `dotnet ef database update -- %CONNECTION_STRING_HERE%` command in _Migrations_ project folder. Don't forget to replace `%CONNECTION_STRING_HERE%` with actual connection string.

__Flow__ works best together with file editors, but they are optional. Apps can always record output to file or print it to STDOUT so you can examine the results of the command without any editor.

## Installation
1. Clone repository
2. Navigate to `src` folder
3. Build solution using `dotnet build` or Visual Studio
4. Navigate to `src\l3\CommandLine\CommandLine.Bundle`
5. Publish bundle to preferrable ouptut directory (ex. `dotnet publish -c Release -o ..\..\..\..\build\cli-bundle`)
6. Navigate to target folder with the bundle
7. run `flow-bundle` to add folder with bundle to path and set appsettings.json as a global config file for all flow apps

Now you can use all CLI tools in any folder.

## Configuration
CLI tools uses [.NET Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration) with [json](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#json-configuration-provider) and [Environment Variables](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#environment-variable-configuration-provider) providers available. 
You can write settings to _appsettings.json_ file in the working directory, specify a separate file location in `FLOW_CONFIG_FILE` environment variable or set environment variables to override anyting what's already in settings file.

There are quite a few settings to provide, and most of them optional:
* `flow:ConnectionString` - required. A connection string to PostgreSQL database that handles data;
* `flow:CultureCode` - optional. Culture code (2-letter language, 2-letter region) that should be used to read or write data. App will use current culture by default;
* `flow:Editor:JSON` - a path to external editor for JSON files. Empty by default;
* `flow:Editor:CSV` - a path to external editor for CSV files. Empty by default;

## Documentation
All CLI tools have `help command` that briefly explains supported commands and its options. More detailed guides can be found below:
* [Expenses Tracking](docs/expenses_tracking.md)
* [Flow Analysis](docs/flow_analysis.md)


## Flow analysis
### Aggregation concepts
_flow_ allows to aggregate your expences and incomes into calendar-like representations:
| Group | Subgroup | jan | feb | mar | ... |
| ----- | -------- | --- | --- | --- | --- |
| Car   | Gas      | 122 | 105 | 70  | ... |
| Car   | Services |     | 50  |     | ... |
| ...   | ........ | ... | ... | ... | ... |

Transactions, stored in the database are getting grouped into the rows based on aggregation criteria defined by the user.
Basically, each row is defined as a set of specific dimension values ('Car' for dimenson 'Group' and 'Gas' or 'Services' for dimension 'Subgroup') and a transaction selection criteria (the same format as for `acountant list` command). Additonaly, _flow_ adds a special group "Everything else" to catch transactions that does not belong to any of the values.

### Defining aggregations
Aggregations must be defined in the following format:
```
Dimension 1 Name; Dimension 2 Name; Dimension N Name
D1 Value 1;D2 Value 1; DN Value 1; selection criteria 1.1
D1 Value 1;D2 Value 1; DN Value 1; selection criteria 1.2
D1 Value 2;D2 Value 2; DN Value 2; selection criteria 2
```
Rows for the same dimension values will be combined into a single row, conditions will be unified using OR operator.

Definitions can be grouped into different groups: then transactions will be evaluated for each group independently:
```
Category;Title
group: totals
Total; Income; a>0
Total; Expense; a<0
group: regular
Home;Rent; t%Rent
Food;Grossery; t%"Grossery store"
```

For each group, user can provide a subgroup - then all items that did not match any of the row from a group will be evaluated for rows from a subgroup:
```
Category;Title
group: regular
Regular;Expenses; cat%regular a<0
Regular;Incomes; cat%regular a>0
subgroup: irregular
Irregular; Expenses; a<0
Irregular; Incomes; a>0
```
Subgroups can also have a subgroup

### Building a calendar
`flow calendar` command builds a calendar from the transactions stored in the database
```
> .\flow help calendar
flow 0.0.1
Copyright (C) 2021 nvsnkv

  -d, --input-dimensions    File with dimensions setup. If specified, app will use it to build calendar dimensions, otherwise app
                            will use standard input.

  -f, --from                Required. Left boundary of date range to aggregate. Transactions with date greater or equal to this
                            value will be included to aggregation.

  -t, --till                Required. Right boundary of date range to aggregate. Transactions with date lesser than this value will
                            be included to aggregation.

  -c, --currency            Required. Target currency. Transactions in different currency will be converted to target currency for
                            proper aggregation.

  -o, --output-file         Output file path.If specified, app will this path to write the calendar, otherwise it will either
                            generate a new file or use standard output depending on configuration.

  --output-format           (Default: CSV) Output format. If output-file is set, output format will be defined by extension of
                            output-file and this option will be ignored.

  -r, --output-rejected     Path to write rejected transactions. If specified, app will use this path to write the list of
                            transactions excluded from aggregation, otherwise it will either generate a new file or use standard
                            output depending on configuration.

  --help                    Display this help screen.

  --version                 Display version information.
```


## Forecast
TBD

## Other noteworthy things
TBD

## Development notes
TBD

## License
not yet defined

