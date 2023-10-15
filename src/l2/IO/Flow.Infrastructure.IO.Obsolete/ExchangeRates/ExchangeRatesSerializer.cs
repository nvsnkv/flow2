using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.ExchangeRates.Contract;
using Flow.Infrastructure.IO.Generics;

namespace Flow.Infrastructure.IO.ExchangeRates;

internal class ExchangeRatesSerializer : IExchangeRatesReader, IExchangeRatesWriter
{
    private readonly CsvSerializer csv;
    private readonly JsonSerializer json;

    public ExchangeRatesSerializer(CsvSerializer csv, JsonSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task<IEnumerable<ExchangeRate>> ReadExchangeRates(StreamReader reader, OldSupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            OldSupportedFormat.CSV => await csv.Read(reader, (ExchangeRateRow r) => (ExchangeRate)r, ct),
            OldSupportedFormat.JSON => await json.Read(reader, (JsonExchangeRate j) => (ExchangeRate)j),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
        };
    }

    public async Task WriteRates(StreamWriter writer, IEnumerable<ExchangeRate> rates, OldSupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case OldSupportedFormat.CSV:
                await csv.Write<ExchangeRate, ExchangeRateRow, ExchangeRateRowMap>(writer, rates, r => (ExchangeRateRow)r, ct);
                break;

            case OldSupportedFormat.JSON:
                await json.Write(writer, rates, ct);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}
