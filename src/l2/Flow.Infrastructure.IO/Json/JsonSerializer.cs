using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonSerializer
{
    private readonly Newtonsoft.Json.JsonSerializer serializer;

    public JsonSerializer(JsonSerializerSettings? settings)
    {
        serializer = settings is null 
            ? Newtonsoft.Json.JsonSerializer.CreateDefault() 
            : Newtonsoft.Json.JsonSerializer.Create(settings);
    }

    public IAsyncEnumerable<T> Read<T, TJson>(StreamReader reader, Func<TJson, T> convertFunc)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<List<TJson>>(jsonReader) ?? Enumerable.Empty<TJson>();

        return result.Select(convertFunc).ToAsyncEnumerable();
    }

    public async Task Write<T>(StreamWriter writer, IAsyncEnumerable<T> transactions, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, await transactions.ToListAsync(ct));
        await jsonWriter.FlushAsync(ct);
    }
}