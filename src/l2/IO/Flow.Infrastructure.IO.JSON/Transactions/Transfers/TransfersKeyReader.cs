using Flow.Domain.Transactions.Transfers;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON.Transactions.Transfers;

internal class TransfersKeyReader : JsonReader<TransferKey, JsonTransferKey>
{
    public TransfersKeyReader(JsonSerializerSettings? settings) : base(settings, j => (TransferKey)j)
    {
    }
}
