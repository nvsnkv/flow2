using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class ExchangeRatesSerializer : IExchangeRatesReader
{
    private readonly CsvSerializer csv;
    private readonly JsonSerializer json;

    public ExchangeRatesSerializer(CsvSerializer csv, JsonSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task<IEnumerable<ExchangeRate>> ReadExchangeRates(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                return await csv.Read(reader,(ExchangeRateRow r) => (ExchangeRate)r, ct);
                
            case SupportedFormat.JSON:
                return await json.Read(reader,(JsonExchangeRate j) => (ExchangeRate)j);
                
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}