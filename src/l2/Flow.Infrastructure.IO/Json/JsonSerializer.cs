﻿using Newtonsoft.Json;

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

    public Task<IEnumerable<T>> Read<T, TJson>(StreamReader reader, Func<TJson, T> convertFunc)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<List<TJson>>(jsonReader) ?? Enumerable.Empty<TJson>();

        return Task.FromResult(result.Select(convertFunc));
    }

    protected async Task Write<T>(StreamWriter writer, IEnumerable<T> transactions, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, transactions);
        await jsonWriter.FlushAsync(ct);
    }
}