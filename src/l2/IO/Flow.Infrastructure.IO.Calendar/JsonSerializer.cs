using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Calendar;

internal class JsonSerializer
{
    private readonly Newtonsoft.Json.JsonSerializer serializer;

    public JsonSerializer(JsonSerializerSettings? settings)
    {
        serializer = settings is null 
            ? Newtonsoft.Json.JsonSerializer.CreateDefault() 
            : Newtonsoft.Json.JsonSerializer.Create(settings);
    }

    public Task<TJson?> Read<TJson>(StreamReader reader)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        return Task.FromResult(serializer.Deserialize<TJson>(jsonReader));
    }

    public async Task Write<T>(StreamWriter writer, IEnumerable<T> transactions, CancellationToken ct)
    {
        await using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, transactions);
        await jsonWriter.FlushAsync(ct);
    }
}
