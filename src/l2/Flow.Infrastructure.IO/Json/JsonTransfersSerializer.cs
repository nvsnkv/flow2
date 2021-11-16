using Flow.Domain.Transactions.Transfers;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonTransfersSerializer: JsonSerializer
{
    public JsonTransfersSerializer(JsonSerializerSettings? settings) : base(settings)
    {
    }

    [Obsolete("Use Read() instead")]
    public async Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader)
    {
        return await Read(reader, (JsonTransferKey j) => (TransferKey)j);
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