using CsvHelper.Configuration;
using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.CSV.ExchangeRates;

[UsedImplicitly]
internal sealed class ExchangeRateRowMap : ClassMap<ExchangeRateRow>
{
    public ExchangeRateRowMap()
    {
        Map(r => r.FROM).Index(0);
        Map(r => r.TO).Index(1);
        Map(r => r.DATE).Index(2);
        Map(r => r.RATE).Index(3);
    }
}