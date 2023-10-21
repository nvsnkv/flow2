using CsvHelper.Configuration;
using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.Transactions.Transfers;

[UsedImplicitly]
internal sealed class TransferRowMap : ClassMap<TransferRow>
{
    public TransferRowMap()
    {
        Map(m => m.SOURCE).Index(0);
        Map(m => m.SINK).Index(1);
        Map(m => m.FEE).Index(2);
        Map(m => m.CURRENCY).Index(3);
        Map(m => m.COMMENT).Index(4);
    }
}