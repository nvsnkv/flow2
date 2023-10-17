using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Common;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;

namespace Flow.Infrastructure.IO.CSV;

internal class CsvRejectionsWriter<TR, TE, TRow> : IFormatSpecificWriter<TR> where TR: RejectedEntity<TE>
{
    private readonly CsvConfiguration config;
    private readonly Func<TR, TRow> convertFunc;

    public CsvRejectionsWriter(CsvConfiguration config, Func<TR, TRow> convertFunc)
    {
        this.config = config;
        this.convertFunc = convertFunc;
    }
    
    public async Task Write(StreamWriter writer, IEnumerable<TR> items, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        foreach (var rejected in items)
        {
            var record = convertFunc(rejected);
            await csvWriter.WriteRecordsAsync(Enumerable.Repeat(record, 1), ct);

            foreach (var reason in rejected.Reasons)
            {
                await writer.WriteLineAsync(reason.ToCharArray(), ct);
            }
        }
    }

    public SupportedFormat Format => CSVIO.SupportedFormat;
}
