using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Generics;

namespace Flow.Infrastructure.IO.ExchangeRates;

internal class RateRejectionsWriter
{
    private readonly CsvRejectionsWriter csv;
    private readonly JsonRejectionsWriter json;
    public RateRejectionsWriter(CsvRejectionsWriter csv, JsonRejectionsWriter json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedRate> rejections, OldSupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case OldSupportedFormat.CSV:
                await csv.Write<RejectedRate, ExchangeRate, ExchangeRateRow>(
                    writer,
                    rejections,
                    r => (ExchangeRateRow)r.Rate,
                    ct);
                return;

            case OldSupportedFormat.JSON:
                await json.WriteRejections(writer, rejections, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}
