using Flow.Domain.Transactions;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO;

internal class JsonTransactionsSerializer
{
    private readonly JsonSerializer serializer;

    public JsonTransactionsSerializer(JsonSerializerSettings? settings)
    {
        this.serializer = settings is null 
            ? JsonSerializer.CreateDefault() 
            : JsonSerializer.Create(settings);
    }

    public Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader)
    {
        var result = Read<Transaction>(reader);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader)
    {
        var result = Read<RecordedTransaction>(reader);
        return Task.FromResult(result);
    }

    private IEnumerable<T> Read<T>(StreamReader reader)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<IEnumerable<T>>(jsonReader) ?? Enumerable.Empty<T>();

        return result;
    }

    public Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions)
    {
        Write<Transaction>(writer, transactions);
        return Task.CompletedTask;
    }

    public Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions)
    {
        Write<RecordedTransaction>(writer, transactions);
        return Task.CompletedTask;
    }

    public Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejected)
    {
        Write(writer, rejected);
        return Task.CompletedTask;
    }

    private void Write<T>(StreamWriter writer, IEnumerable<T> transactions)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, transactions);
    }
}