using Flow.Domain.Analysis;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON.Transactions;

internal class TaggedTransactionsReader : JsonReader<TaggedTransaction, JsonTaggedTransaction>
{
    public TaggedTransactionsReader(JsonSerializerSettings? settings) : base(settings, j => (TaggedTransaction)j)
    {
    }
}