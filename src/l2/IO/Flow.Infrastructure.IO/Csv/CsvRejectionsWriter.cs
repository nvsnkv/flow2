using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Common;

namespace Flow.Infrastructure.IO.Csv;

internal class CsvRejectionsWriter
{
    private readonly CsvConfiguration config;

    public CsvRejectionsWriter(CsvConfiguration config)
    {
        this.config = config;
    }
    
    public async Task Write<TR, TE, TRow>(StreamWriter writer, IEnumerable<TR> rejections, Func<TR, TRow> convertFunc, CancellationToken ct) where TR : RejectedEntity<TE>
    {
        await using var csvWriter = new CsvWriter(writer, config);
        foreach (var rejected in rejections)
        {
            var record = convertFunc(rejected);
            await csvWriter.WriteRecordsAsync(Enumerable.Repeat(record, 1), ct);

            foreach (var reason in rejected.Reasons)
            {
                await writer.WriteLineAsync(reason.ToCharArray(), ct);
            }
        }
    }
}