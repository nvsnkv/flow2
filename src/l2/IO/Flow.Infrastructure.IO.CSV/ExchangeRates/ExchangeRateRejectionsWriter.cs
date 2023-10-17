using CsvHelper.Configuration;
using Flow.Domain.ExchangeRates;

namespace Flow.Infrastructure.IO.CSV.ExchangeRates;

internal class ExchangeRateRejectionsWriter : CsvRejectionsWriter<RejectedRate, ExchangeRate, ExchangeRateRow>
{
    public ExchangeRateRejectionsWriter(CsvConfiguration config) : base(config, r => (ExchangeRateRow)r.Rate)
    {
    }
}