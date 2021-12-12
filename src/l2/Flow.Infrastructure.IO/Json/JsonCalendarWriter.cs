using System.Collections.ObjectModel;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonCalendarWriter
{
    private readonly Newtonsoft.Json.JsonSerializer serializer;

    public JsonCalendarWriter(JsonSerializerSettings? settings)
    {
        serializer = settings is null
            ? Newtonsoft.Json.JsonSerializer.CreateDefault()
            : Newtonsoft.Json.JsonSerializer.Create(settings);
    }

    public async Task Write(StreamWriter writer, Calendar calendar, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        var rows = calendar.Values.ToDictionary(
            v => v.Key, 
            v => (IReadOnlyList<Aggregate>)v.Value
                .Select(a => new Aggregate(a.Value, a.Transactions.Select(t => (RecordedTransaction)(JsonRecordedTransaction)t).ToList().AsReadOnly()))
                .ToList()
                .AsReadOnly()
            );

        calendar = new Calendar(calendar.Ranges, calendar.Dimensions,  new ReadOnlyDictionary<Vector, IReadOnlyList<Aggregate>>(rows));
        serializer.Serialize(jsonWriter, calendar);
        await jsonWriter.FlushAsync(ct);
    }
}