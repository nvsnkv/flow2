using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class ExchangeRatesSerializer : IExchangeRatesReader, IExchangeRatesWriter
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
        return format switch
        {
            SupportedFormat.CSV => await csv.Read(reader, (ExchangeRateRow r) => (ExchangeRate)r, ct),
            SupportedFormat.JSON => await json.Read(reader, (JsonExchangeRate j) => (ExchangeRate)j),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
        };
    }

    public async Task WriteRates(StreamWriter writer, IEnumerable<ExchangeRate> rates, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.Write<ExchangeRate, ExchangeRateRow, ExchangeRateRowMap>(writer, rates, r => (ExchangeRateRow)r, ct);
                break;

            case SupportedFormat.JSON:
                await json.Write(writer, rates, ct);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}