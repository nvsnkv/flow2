using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonTransactionsSerializerBase
{
    private readonly JsonSerializer serializer;

    public JsonTransactionsSerializerBase(JsonSerializerSettings? settings)
    {
        serializer = settings is null 
            ? JsonSerializer.CreateDefault() 
            : JsonSerializer.Create(settings);
    }

    protected IEnumerable<T> Read<T>(StreamReader reader)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<List<T>>(jsonReader) ?? Enumerable.Empty<T>();

        return result;
    }

    protected async Task Write<T>(StreamWriter writer, IEnumerable<T> transactions, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, transactions);
        await jsonWriter.FlushAsync(ct);
    }
}