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
Subgroups can also have a subgroup.

### Building a calendar
`flow calendar` command builds a calendar from the transactions stored in the database. It requires the dimensions setup, left and right boundaries for the calendar and target currency. Please refere to `flow help calendar` output for details.
