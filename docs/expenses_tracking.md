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

`accountant edit` would be useful if you need to retrive the data from database and update it in one pass. Basically it's a combination of `list` and `update` commands.

### Transfers
Flow automatically detects various occurences of money transfers between accounts. That allows to exclude them from list of really valuable transactions and simplify analysis.

To check correctness of transfer detection algorythms you can use `transfers list` command. Like `accountant list`, it accepts the search criteria for the set of transactions in which app will try to find transfers.
For each transfer tool returns Source (a Key of source transaction), Sink (a Key of sink transaction), Fee (difference between source and sink), Currency in which Fee was calculated and a Comment that explains why tool believe it's a transfer.

If necessary user can enforse a particular transfer by providing a pair of Source and Sink to `transfers enforce` command. Previously enforsed transfers can be abandoned using `transfers abandon` command.

Flow comes with the logic that can find potential transfers - usually cross-bank transfer operations gets logged with different titles in source and sink. As a result, system cannot match them automatically.
`transfers guess` command will provide the list of hypotetical transfers. Output of this command can be used to build the list of transfers to enforce with `transfers enforce`

### Exchange Rates
Where needed, Flow automatically requests exchange rates to perform conversion. Rates that were successfully received from the Central Bank ofr the Russian Federation are getting cached in local storage to speed up subsequent calculations.

You can use `rates list` command to check which rates were already captured.
