using Flow.Domain.Transactions;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON.Transactions;

internal class TransactionsReader : JsonReader<Transaction, JsonTransaction>
{
    public TransactionsReader(JsonSerializerSettings? settings) : base(settings, j => (Transaction)j)
    {
    }
}
