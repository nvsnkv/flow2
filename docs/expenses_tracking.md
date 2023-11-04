## Expences tracking
`flow-tx.exe` is the tool that helps to manage information about accounts and transactions.
### Transactions management

#### Basic
Flow allows to capture following information about transactions: Timestamp, Amount, Currency, Category, Title, Account Name, Bank Name.
Once stored, every transaction receives a unique Key value and user receives an opportunity to provide a Note, and overrides for Title and Category.
Overrides will replace original values on flow lists.

All information can be exported or imported in one of 2 supported formats: CSV or JSON.

`flow tx list` is the command it executes by default. It exports recorded transactions that meets input criteria. The criteria allows to filter by 
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

Following operations are supported:
* equality: `=`
* likeness: `%`
* comparison: `<`, `>` , `>=`, `<=`
* in-range: `[]`, `[)`, `(]` `()`
* within: `(1, 2, 14)`

For example, `flow tx list ts[2021-01-01:2021-10-01)` will return transactions with start date greater or equal to 2021-01-01 and strictly less than 2021-10-01. Another example, `k>200 a<0` will return all transaction with Key > 200 and negative Amount.

Transactions can be inserted using `flow tx add` command, edited using `flow tx update` and deleted by `flow tx delete` command. Please refer to help screens for each of the commands for additional details.

`flow tx edit` would be useful if you need to retrive the data from database and update it in one pass. Basically it's a combination of `list` and `update` commands.

#### Duplicates detection
Sometimes banks make adjustments during the processing that may move transaction date from, let's say, weekend to the next Monday. As a result, the same transaction will receive different dates in account statements as of Sunday and as of Monday. That may lead to unexpected duplication.

`flow tx list` accepts `-d` or `--duplicates` key to list only (potentially) duplicated records. This key can be extended by `-r, --duplicates-range` parameter that specifies the date range between earliest and latest duplicated rows.
Application will threat records as duplicate if all fields except overrides, key and timestamp are the same and the timestamps are "close enough".

### Transfers
Flow automatically detects various occurences of money transfers between accounts. That allows to exclude them from list of really valuable transactions and simplify analysis.

To check correctness of transfer detection algorythms you can use `flow xfrs list` command. Like `flow tx list`, it accepts the search criteria for the set of transactions in which app will try to find transfers.
For each transfer tool returns Source (a Key of source transaction), Sink (a Key of sink transaction), Fee (difference between source and sink), Currency in which Fee was calculated and a Comment that explains why tool believe it's a transfer.

If necessary user can enforse a particular transfer by providing a pair of Source and Sink to `flow xfrs enforce` command. Previously enforsed transfers can be abandoned using `flow xfrs abandon` command.

Flow comes with the logic that can find potential transfers - usually cross-bank transfer operations gets logged with different titles in source and sink. As a result, system cannot match them automatically.
`transfers guess` command will provide the list of hypotetical transfers. Output of this command can be used to build the list of transfers to enforce with `transfers enforce`

### Import process

`flow import` commands family allows to spped up import process. You need to put all files with transactions into single folder and run `flow import start` in this folder.
Then flow will:
1. Identify file format from file extension (e.g. `.csv`, `.json` or `.my-custom-format.csv` in case you have plugins to read it) and add all transactions from these files to the system in a same way `flow tx add` adds. Rejected transactions will be saved to separate file for each input file.
2. Find possible duplicates and save them into separate file for further investigation
3. Find possible transfers and save them into separate file for further investigation

After import you can:
* edit transactions using `flow import edit`
* abort import process and remove added transactions using `flow import abort`
* complete import using `flow import complete`

You can remove duplicates using `flow tx delete` any time. Please note that this action is irreversible, `flow import abort` won't restore removed transactions!

You can enforce transfers using `flow xfers enforce` any time.

### Exchange Rates
Where needed, Flow automatically requests exchange rates to perform conversion. Rates that were successfully received from the Central Bank ofr the Russian Federation are getting cached in local storage to speed up subsequent calculations.

You can use `rates list` command to check which rates were already captured.
