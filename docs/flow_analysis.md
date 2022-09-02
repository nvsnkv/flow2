## Flow analysis
### Aggregation concepts
_flow_ allows to aggregate your expences and incomes into calendar-like representations:
| Group | Subgroup | jan | feb | mar | ... |
| ----- | -------- | --- | --- | --- | --- |
| Car   | Gas      | 122 | 105 | 70  | ... |
| Car   | Services |     | 50  |     | ... |
| ...   | ........ | ... | ... | ... | ... |

Transactions, stored in the database are getting grouped into the rows based on aggregation criteria defined by the user.
Basically, each row is defined as a set of specific dimension values ('Car' for dimenson 'Group' and 'Gas' or 'Services' for dimension 'Subgroup') and a transaction selection criteria (the same format as for `flow tx list` command). Transactions that does not belong to any of configured group will be reported as rejected.
_flow_ allows multi-level aggregation, for example you can
* Group all your grocery stores in one row (Grocery)
* Group car-related expences (Car) into 
	* Gas ( , Gas)
	* and Services ( , Services)
* Introduce "Everything else" group (Else)
	* and define a subgroups based on category and title of transactions. ( , Category, Title)

In example above, _flow_ will create a row for each of the bullet. Rows with subgroups (Car or Else) will have both row with summary and the row for each of the subgroup defined:
|         |          |           | jan  | feb  | mar  | ... |
| ------- | -------- | --------- | ---- | ---- | ---- | --- |
| Grocery |          |           | -126 | -156 | -135 | ... |
| Car     |          |           | -80  | -120 | -50  | ... |
|         | Gas      |           | -50  | -50  | -50  | ... |
|         | Serivces |           | -30  | -70  |      | ... |
| Else    |          |           | -200 | -198 | -145 | ... |
|         | Fastfood | MCDonalds | -30  | -30  | -40  | ... |
|         | Fastfood | KFC       | -15  | -30  | -2   | ... |
| ...     |  .....   | ......    | ...  | ...  | ...  | ... |

### Defining aggregations
Aggregations can be defined using the following JSON object: 
```
"Dimensions":[], /* the list of headers for aggregation. Flow will pad this list with emptry string to match with the largest Measurement definition from the Series below */
    "Series":[ /* collection of series definition */
        {
            "Measurement": ["Groccery"],  /*List that defines a particular dimension of a calendar. Supports substitutions. */
            "Rules":["cat='Grocery store'"] /* List of transaction selection criteria in a same format as for transactions management commands or special ELSE keyword */
            /* "Rule": null, - a short alternative to Rules array when series has only one rule. */
            /* "SubSeries" : [] - optional list of subseries that helps to analize a bigger series */
        }
        {
            "Measurement": ["Car"],
            "Rules": [ "t%Gas", "'t%STO'" ]
            ],
            "SubSeries": [
                {
                    "Measurement": ["","Gas"],
                    "Rule": "t%Gas"
                },
                {
                    "Measurement": ["", "Services"],
                    "Rule": "t%STO"
                }
            ]
        },
        {
            "Measurement": ["Else"],
            "Rule": "ELSE" /* matches with any transaction given */
            ],
            "SubSeries": [
                {
                    "Measurement": ["", "$cat","$t"], /* flow will substitute second value with Category of transaction and third value with Title. All transactions with the same title and category will fall into the same subseries */
                    "Rule": "ELSE"
                }
            ]
        }
    ]
}
```

### Building a calendar
`flow core calendar` command builds a calendar from the transactions stored in the database. It requires the dimensions setup, left and right boundaries for the calendar and target currency. Please refere to `flow core help calendar` output for details.
