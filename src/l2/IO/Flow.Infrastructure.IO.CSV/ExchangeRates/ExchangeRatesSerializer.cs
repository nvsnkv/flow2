using CsvHelper.Configuration;
using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;

namespace Flow.Infrastructure.IO.CSV.ExchangeRates;

internal sealed class ExchangeRatesReader : CsvReader<ExchangeRate, ExchangeRateRow>
{
    public ExchangeRatesReader(CsvConfiguration config) : base(config, r => (ExchangeRate)r)
    {
    }
}
