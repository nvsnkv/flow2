using Flow.Domain.Transactions;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON.Transactions;

internal class RecordedTransactionsReader : JsonReader<RecordedTransaction, JsonRecordedTransaction>
{
    public RecordedTransactionsReader(JsonSerializerSettings? settings) : base(settings, j => (RecordedTransaction)j)
    {
    }
}
