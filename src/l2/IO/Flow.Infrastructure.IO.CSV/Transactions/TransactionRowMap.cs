using CsvHelper.Configuration;
using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.CSV.Transactions;

[UsedImplicitly]
internal sealed class TransactionRowMap : ClassMap<TransactionRow>
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
        Map(m => m.COMMENT).Index(7);
        Map(m => m.CATEGORY_OVERRIDE).Index(8);
        Map(m => m.TITLE_OVERRIDE).Index(9);
    }
}