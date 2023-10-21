using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Contract;
using Flow.Plugins.Transactions.FlexibleTransactionsReader.Settings;

namespace Flow.Plugins.Transactions.FlexibleTransactionsReader.Impl;

internal sealed class FlexibleTransactionsReader : IFlexibleTransactionsReader
{
    private readonly CultureInfo culture;
    private readonly ClassMap<InputDataRow> map;
    public FlexibleTransactionsReader(CultureInfo culture, string formatName, IReadOnlyCollection<MappingRule>? rules)
    {
        this.culture = culture;
        Format = new(formatName);
        map = new InputDataRowClassMap(rules);
    }

    public async Task<IEnumerable<IncomingTransaction>> Read(StreamReader reader, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, new CsvConfiguration(culture));
        csvReader.Context.RegisterClassMap(map);

        return await csvReader.GetRecordsAsync(typeof(InputDataRow), ct)
            .Where(r => r != null)
            .Select(row =>
            {
                var (t, r) = (InputDataRow)row!;
                return (t, r);
            })
            .ToListAsync(ct);
    }

    public SupportedFormat Format { get; }
}
