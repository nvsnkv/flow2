using CsvHelper.Configuration;
using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.CSV.Transactions;

[UsedImplicitly]
internal sealed class RecordedTransactionRowMap : ClassMap<RecordedTransactionRow>
{
    public RecordedTransactionRowMap()
    {
        Map(m => m.KEY).Index(0);
        Map(m => m.TIMESTAMP).Index(1);
        Map(m => m.AMOUNT).Index(2);
        Map(m => m.CURRENCY).Index(3);
        Map(m => m.CATEGORY).Index(4);
        Map(m => m.TITLE).Index(5);
        Map(m => m.ACCOUNT).Index(6);
        Map(m => m.BANK).Index(7);
        Map(m => m.COMMENT).Index(8);
        Map(m => m.CATEGORY_OVERRIDE).Index(9);
        Map(m => m.TITLE_OVERRIDE).Index(10);
        Map(m => m.REVISION).Index(11);
    }
}
