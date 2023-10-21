using CsvHelper.Configuration;
using Flow.Domain.ExchangeRates;

namespace Flow.Infrastructure.IO.CSV.ExchangeRates;

internal class RejectedRatesWriter : CsvRejectionsWriter<RejectedRate, ExchangeRate, ExchangeRateRow>
{
    public RejectedRatesWriter(CsvConfiguration config) : base(config, r => (ExchangeRateRow)r.Rate)
    {
    }
}