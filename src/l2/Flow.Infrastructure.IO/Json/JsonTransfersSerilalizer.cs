using Flow.Domain.Transactions.Transfers;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonTransfersSerilalizer: JsonTransactionsSerializerBase
{
    public JsonTransfersSerilalizer(JsonSerializerSettings? settings) : base(settings)
    {
    }

    public Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader)
    {
        var result = Read<JsonTransferKey>(reader).Select(j => (TransferKey)j);
        return Task.FromResult(result);
    }

    public async Task WriteTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, CancellationToken ct)
    {
        await Write(writer, keys, ct);
    }

    public async Task WriteTransfers(StreamWriter writer, IEnumerable<Transfer> transfers, CancellationToken ct)
    {
        await Write(writer, transfers, ct);
    }
}