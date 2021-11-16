using Flow.Domain.Transactions;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonTransactionsSerializer : JsonSerializer
{
    public JsonTransactionsSerializer(JsonSerializerSettings? settings) : base(settings)
    {
    }
    
    [Obsolete("Use Read() instead")]
    public async Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader)
    {
        return await Read(reader,  (JsonTransaction j) => (Transaction)j);
        
    }

    [Obsolete("Use Read() instead")]
    public async Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader)
    {
        return await Read(reader, (JsonRecordedTransaction j) => (RecordedTransaction)j);
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }
}