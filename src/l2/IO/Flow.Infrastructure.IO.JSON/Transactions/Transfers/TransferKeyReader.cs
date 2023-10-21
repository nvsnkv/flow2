using Flow.Domain.Transactions.Transfers;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON.Transactions.Transfers;

internal class TransferKeyReader : JsonReader<TransferKey, JsonTransferKey>
{
    public TransferKeyReader(JsonSerializerSettings? settings) : base(settings, j => (TransferKey)j)
    {
    }
}
