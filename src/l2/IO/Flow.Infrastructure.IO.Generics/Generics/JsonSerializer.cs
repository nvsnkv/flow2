﻿using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Generics;

public class JsonSerializer
{


    public Task<IEnumerable<T>> Read<T, TJson>(StreamReader reader, Func<TJson, T> convertFunc)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<List<TJson>>(jsonReader) ?? Enumerable.Empty<TJson>();

        return Task.FromResult(result.Select(convertFunc));
    }

    public Task<TJson?> Read<TJson>(StreamReader reader)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        return Task.FromResult(serializer.Deserialize<TJson>(jsonReader));
    }

    public async Task Write<T>(StreamWriter writer, IEnumerable<T> transactions, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, transactions);
        await jsonWriter.FlushAsync(ct);
    }

    public async Task Write<T>(StreamWriter writer, IAsyncEnumerable<T> transactions, CancellationToken ct)
    {
        await Write(writer, await transactions.ToListAsync(ct), ct);
    }
}
