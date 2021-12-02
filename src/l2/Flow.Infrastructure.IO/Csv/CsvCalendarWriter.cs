﻿using CsvHelper;
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
        await csvWriter.WriteRecordsAsync(headerRow, ct);

        foreach (var value in calendar.Values)
        {
            if (ct.IsCancellationRequested) { return; }

            var row = GetRow(value.Key, value.Value);
            await csvWriter.WriteRecordsAsync(row, ct);
        }
    }
    
    private IEnumerable<object?> GetRow<T>(Vector vector, IEnumerable<T> rangedData, Func<T, object>? selectorFunc = null)
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