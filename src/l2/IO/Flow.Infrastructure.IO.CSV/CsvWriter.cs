using CsvHelper;
using CsvHelper.Configuration;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;

namespace Flow.Infrastructure.IO.CSV;

internal class CsvWriter<T, TRow, TMap> : IFormatSpecificWriter<T> where TMap : ClassMap
{
    private readonly CsvConfiguration config;
    private readonly Func<T, TRow> convertFunc;

    public CsvWriter(CsvConfiguration config, Func<T, TRow> convertFunc)
    {
        this.config = config;
        this.convertFunc = convertFunc;
    }

    public async Task Write(StreamWriter writer, IEnumerable<T> records, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config, true);
        csvWriter.Context.RegisterClassMap<TMap>();

        await csvWriter.WriteRecordsAsync(records.Select(convertFunc), ct);
    }

    public SupportedFormat Format => CSVIO.SupportedFormat;
}