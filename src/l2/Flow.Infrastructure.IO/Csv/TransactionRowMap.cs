using System.Linq.Expressions;
using CsvHelper.Configuration;

namespace Flow.Infrastructure.IO.Csv;

internal class TransactionRowMap : ClassMap<TransactionRow>
{
    public TransactionRowMap()
    {
        Map(m => m.TIMESTAMP).Index(0);
        Map(m => m.AMOUNT).Index(1);
        Map(m => m.CURRENCY).Index(2);
        Map(m => m.CATEGORY).Index(3);
        Map(m => m.TITLE).Index(4);
        Map(m => m.ACCOUNT).Index(5);
        Map(m => m.BANK).Index(6);
    }
}