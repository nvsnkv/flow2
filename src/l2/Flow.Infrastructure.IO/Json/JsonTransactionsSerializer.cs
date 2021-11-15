using Flow.Domain.Transactions;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonTransactionsSerializer : JsonTransactionsSerializerBase
{
    public JsonTransactionsSerializer(JsonSerializerSettings? settings) : base(settings)
    {
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

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }
}