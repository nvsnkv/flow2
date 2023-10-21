using CsvHelper;
using CsvHelper.Configuration;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;

namespace Flow.Infrastructure.IO.CSV;

internal class CsvReader<T, TRow> : IFormatSpecificReader<T>
{
    private readonly CsvConfiguration config;
    private readonly Func<TRow, T> convertFunc;

    public CsvReader(CsvConfiguration config, Func<TRow, T> convertFunc)
    {
        this.config = config;
        this.convertFunc = convertFunc;
    }

    public async Task<IEnumerable<T>> Read(StreamReader reader, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, config);

        return await csvReader.GetRecordsAsync(typeof(TRow), ct).Select(row => convertFunc((TRow)row!)).ToListAsync(ct);
    }

    public SupportedFormat Format => CSVIO.SupportedFormat;
}
