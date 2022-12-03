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

    public async Task<IEnumerable<T>> Read<T, TRow>(StreamReader reader, Func<TRow, T> convertFunc, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, config);

        return await csvReader.GetRecordsAsync(typeof(TRow), ct).Select(row => convertFunc((TRow)row!)).ToListAsync(ct);
    }

    public async Task Write<T, TRow, TMap>(StreamWriter writer, IEnumerable<T> records, Func<T, TRow> convertFunc, CancellationToken ct) where TMap : ClassMap
    {
        await Write<T, TRow, TMap>(writer, records.ToAsyncEnumerable(), convertFunc, ct);
    }
    
    public async Task Write<T, TRow, TMap>(StreamWriter writer, IAsyncEnumerable<T> records, Func<T, TRow> convertFunc, CancellationToken ct) where TMap : ClassMap
    {
        await using var csvWriter = new CsvWriter(writer, config, true);
        csvWriter.Context.RegisterClassMap<TMap>();

        await csvWriter.WriteRecordsAsync(records.Select(convertFunc), ct);
    }

    public async Task Write<T, TRow>(StreamWriter writer, IEnumerable<T> records, Func<T, TRow> convertFunc, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config, true);

        await csvWriter.WriteRecordsAsync(records.Select(convertFunc), ct);
    }
}