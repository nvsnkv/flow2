using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.JSON.Contract;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON;

internal class JsonWriter<T> : IFormatSpecificWriter<T>
{
    private readonly JsonSerializer serializer;

    public JsonWriter(JsonSerializerSettings? settings)
    {
        serializer = settings is null
            ? JsonSerializer.CreateDefault()
            : JsonSerializer.Create(settings);
    }


    public async Task Write(StreamWriter writer, IEnumerable<T> items, CancellationToken ct)
    {
        await using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, items);
        await jsonWriter.FlushAsync(ct);
    }

    public SupportedFormat Format => JSONIO.SupportedFormat;
}