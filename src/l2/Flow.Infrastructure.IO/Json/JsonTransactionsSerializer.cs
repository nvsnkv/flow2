using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
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

    public Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader)
    {
        var result = Read<JsonTransferKey>(reader).Select(j => (TransferKey)j);
        return Task.FromResult(result);
    }

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, CancellationToken ct)
    {
        await Write(writer, rejections, ct);
    }

    public async Task WriterTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, CancellationToken ct)
    {
        await Write(writer, keys, ct);
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransferKey> rejections, CancellationToken ct)
    {
        await Write(writer, rejections, ct);
    }

    public async Task WriterTransfers(StreamWriter writer, IEnumerable<Transfer> transfers, CancellationToken ct)
    {
        await Write(writer, transfers, ct);
    }

    private IEnumerable<T> Read<T>(StreamReader reader)
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<List<T>>(jsonReader) ?? Enumerable.Empty<T>();

        return result;
    }

    private async Task Write<T>(StreamWriter writer, IEnumerable<T> transactions, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, transactions);
        await jsonWriter.FlushAsync(ct);
    }
}