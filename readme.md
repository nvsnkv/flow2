# Flow - an expense tracking tools
__Flow__ is a set of command line tools designed to simplify expences tracking, analysis and forecast. Working in conjuction with your favorite editors for JSON and CSV files it helps to maintain your budget.

# How to use it?
## Prerequisites
__Flow__  works with a PostgreSQL database, so you'll need to spin one up and save connection string into flow configuration. Before the first use, you need to apply database migrations from `Flow.Infrastructure.Storage.Migrations` project using [EF Core .NET Core CLI tools](https://docs.microsoft.com/en-us/ef/core/get-started/overview/install#get-the-net-core-cli-tools). They are already referenced in `src\.config\dotnet-tools.json` so you'll just need to run `dotnet tool update` as mentioned in the guide.
Once tools are installed, please use `dotnet ef database update -- %CONNECTION_STRING_HERE%` command in _Migrations_ project folder. Don't forget to replace `%CONNECTION_STRING_HERE%` with actual connection string.

__Flow__ works best together with file editors, but they are optional. Apps can always record output to file or print it to STDOUT so you can examine the results of the command without any editor.

## Configuration
CLI tools uses [.NET Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration) with [json](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#json-configuration-provider) and [Environment Variables](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#environment-variable-configuration-provider) providers available. 
You can write settings to _appsettings.json_ file in the working directory, specify a separate file location in `FLOW_CONFIG_FILE` environment variable or set environment variables to override anyting what's already in settings file.

There are quite a few settings to provide, and most of them optional:
* `flow:ConnectionString` - required. A connection string to PostgreSQL database that handles data;
* `flow:CultureCode` - optional. Culture code (2-letter language, 2-letter region) that should be used to read or write data. App will use current culture by default;
* `flow:Editor:JSON` - a path to external editor for JSON files. Empty by default;
* `flow:Editor:CSV` - a path to external editor for CSV files. Empty by default;

## Expences tracking
`accountant.exe` is the tool that helps to manage information about accounts and transactions.
### Transactions management
Flow allows to capturee following information about transactions: Timestamp, Amount, Currency, Category, Title, Account Name, Bank Name.
Once stored, every transaction receives a unique Key value and user receives an opportunity to provide a Note, and overrides for Title and Category.
Overrides will replace original values on flow lists.

All information can be exported or imported in one of 2 supported formats: CSV or JSON.

`accountant list` is the command accountant executes by default. It exports recorded transactions that meets input criteria. The criteria allows to filter by 
* `k` - Key
* `ts` - Timestamp
* `a` - Amount
* `c` - Currency
* `cat` - Category
* `t` - Title
* `acc` - Account name
* `bnk` - Bank
* `ocomm` - Comment 
* `ocat` - Category override
* `ottl` - Title override

Following operation are supported:
* equality: `=`
* likeness: `~`
* comparison: `<`, `>` , `>=`, `<=`
* in-range: `[]`, `[)`, `(]` `()`
* within: `(1, 2, 14)`

For example, `accountant list ts[2021-01-01:2021-10-01)` will return transactions with start date greater or equal to 2021-01-01 and strictly less than 2021-10-01. Another example, `k>200 a<0` will return all transaction with Key > 200 and negative Amount.

Transactions can be inserted using `accountant add` command, edited using `accountant update` and deleted by `accountant delete` command. Please refer to help screens for each of the commands for additional details.

### Transfers
Flow automatically detects various occurences of money transfers between accounts. That allows to exclude them from list of really valuable transactions and simplify analysis.

To check correctness of transfer detection algorythms you can use `transfers list` command. Like `accountant list`, it accepts the search criteria for the set of transactions in which app will try to find transfers.
For each transfer tool returns Source (a Key of source transaction), Sink (a Key of sink transaction), Fee (difference between source and sink), Currency in which Fee was calculated and a Comment that explains why tool believe it's a transfer.

If necessary user can enforse a particular transfer by providing a pair of Source and Sink to `transfers enforce` command. Previously enforsed transfers can be abandoned using `transfers abandon` command.

### Exchange Rates
Where needed, _flow_ automatically requests exchange rates to perform conversion. Rates that were successfully received from the Central Bank ofr the Russian Federation are getting cached in local storage to speed up subsequent calculations.

You can use `rates list` command to check which rates were already captured.

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

