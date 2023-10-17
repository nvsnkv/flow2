using Flow.Domain.Analysis;
using Flow.Domain.Transactions;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Calendar;

internal class JsonCalendarWriter
{
    private readonly Newtonsoft.Json.JsonSerializer serializer;

    public JsonCalendarWriter(JsonSerializerSettings? settings)
    {
        serializer = settings is null
            ? Newtonsoft.Json.JsonSerializer.CreateDefault()
            : Newtonsoft.Json.JsonSerializer.Create(settings);
    }

    public async Task Write(StreamWriter writer, Domain.Analysis.Calendar calendar, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        var series = calendar.Series
            .Select(s => new Series(
                s.Measurement,
                s.Values
                    .Select(
                        v => new Aggregate(v.Value, v.Transactions.Select(t => (RecordedTransaction)(JsonRecordedTransaction)t)))
                    .ToList()
                )
            )
            .ToList();

        calendar = new Domain.Analysis.Calendar(calendar.Ranges, calendar.Dimensions, series);
        serializer.Serialize(jsonWriter, calendar);
        await jsonWriter.FlushAsync(ct);
    }
}