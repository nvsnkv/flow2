using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Analysis;

namespace Flow.Infrastructure.IO.Csv;

internal class CsvCalendarWriter
{
    private readonly CsvConfiguration config;

    public CsvCalendarWriter(CsvConfiguration config)
    {
        this.config = config;
    }

    public async Task Write(StreamWriter writer, Calendar calendar, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        var headerRow = GetRow(calendar.Dimensions, calendar.Ranges, r => r.Alias);
        foreach (var o in headerRow)
        {
            if (ct.IsCancellationRequested) { return; }
            csvWriter.WriteField(o);
        }
        await csvWriter.NextRecordAsync();

        foreach (var series in calendar.Series)
        {
            if (ct.IsCancellationRequested) { return; }

            var row = GetRow(series.Measurement, series.Values, a => a.Value).ToList();
            foreach (var o in row)
            {
                if (ct.IsCancellationRequested) { return; }
                csvWriter.WriteField(o);
            }

            await csvWriter.NextRecordAsync();
        }
    }
    
    private IEnumerable<object?> GetRow<T>(IEnumerable<string> vector, IEnumerable<T> rangedData, Func<T, object?>? selectorFunc = null)
    {
        foreach (var item in vector)
        {
            yield return item;
        }

        foreach (var data in rangedData)
        {
            yield return selectorFunc != null
                ? selectorFunc(data)
                : data;
        }
    }
}