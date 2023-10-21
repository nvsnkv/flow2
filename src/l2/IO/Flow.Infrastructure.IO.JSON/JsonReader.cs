using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.JSON.Contract;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON;

internal class JsonReader<T,TJson> : IFormatSpecificReader<T>
{
    private readonly Func<TJson, T> convertFunc;
    private readonly JsonSerializer serializer;

    public JsonReader(JsonSerializerSettings? settings, Func<TJson, T> convertFunc)
    {
        this.convertFunc = convertFunc;
        serializer = settings is null
            ? JsonSerializer.CreateDefault()
            : JsonSerializer.Create(settings);
    }

    public Task<IEnumerable<T>> Read(StreamReader reader, CancellationToken ct)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<List<TJson>>(jsonReader) ?? Enumerable.Empty<TJson>();

        return Task.FromResult(result.Select(convertFunc));
    }

    public SupportedFormat Format => JSONIO.SupportedFormat;
}
