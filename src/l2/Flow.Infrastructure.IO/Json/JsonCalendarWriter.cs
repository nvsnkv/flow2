using Flow.Domain.Analysis;
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
        serializer.Serialize(jsonWriter, calendar);
        await jsonWriter.FlushAsync(ct);
    }
}