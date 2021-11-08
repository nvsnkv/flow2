using Flow.Domain.Transactions;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonTransactionsSerializer
{
    private readonly JsonSerializer serializer;

    public JsonTransactionsSerializer(JsonSerializerSettings? settings)
    {
        serializer = settings is null 
            ? JsonSerializer.CreateDefault() 
            : JsonSerializer.Create(settings);
    }

    public Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader)
    {
        var result = Read<JsonTransaction>(reader).Select(j => (Transaction)j);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader)
    {
        var result = Read<JsonRecordedTransaction>(reader).Select(j => (RecordedTransaction)j);
        return Task.FromResult(result);
    }

    private IEnumerable<T> Read<T>(StreamReader reader) where T :JsonTransaction
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<List<T>>(jsonReader) ?? Enumerable.Empty<T>();

        return result;
    }

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejected, CancellationToken ct)
    {
        await Write(writer, rejected, ct);
    }

    private async Task Write<T>(StreamWriter writer, IEnumerable<T> transactions, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, transactions);
        await jsonWriter.FlushAsync(ct);
    }
}