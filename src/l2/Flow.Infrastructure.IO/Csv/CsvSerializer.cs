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
        var result = new List<T>();

        await foreach (var row in csvReader.GetRecordsAsync(typeof(TRow), ct))
        {
            result.Add(convertFunc((TRow)row));
        }

        return result;
    }
}