using CsvHelper.Configuration;
using Flow.Domain.ExchangeRates;

namespace Flow.Infrastructure.IO.CSV.ExchangeRates;

internal sealed class ExchangeRatesWriter : CsvWriter<ExchangeRate, ExchangeRateRow, ExchangeRateRowMap>
{
    public ExchangeRatesWriter(CsvConfiguration config) : base(config, r => (ExchangeRateRow)r)
    {
    }
}
