using CsvHelper;
using CsvHelper.Configuration;

namespace Flow.Infrastructure.IO.Csv;

internal class CsvSerializer
{
    private readonly CsvConfiguration config;

    public CsvSerializer(CsvConfiguration config)
    {
        this.config = config;
    }

    public IAsyncEnumerable<T> Read<T, TRow>(StreamReader reader, Func<TRow, T> convertFunc, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, config);

        return csvReader.GetRecordsAsync(typeof(TRow), ct).Select(r => convertFunc((TRow)r));
    }

    public async Task Write<T, TRow, TMap>(StreamWriter writer, IAsyncEnumerable<T> records, Func<T, TRow> convertFunc, CancellationToken ct) where TMap : ClassMap
    {
        await using var csvWriter = new CsvWriter(writer, config);
        csvWriter.Context.RegisterClassMap<TMap>();

        await csvWriter.WriteRecordsAsync(records.Select(convertFunc), ct);
    }

    public async Task Write<T, TRow>(StreamWriter writer, IAsyncEnumerable<T> records, Func<T, TRow> convertFunc, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);

        await csvWriter.WriteRecordsAsync(records.Select(convertFunc), ct);
    }
}