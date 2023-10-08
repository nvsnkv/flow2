using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Transactions;

internal class JsonTransferKey
{
    public long? SourceKey { get; set; }

    public long? SinkKey { get; set; }

    public static explicit operator TransferKey(JsonTransferKey js)
    {
        return new TransferKey(js.SourceKey ?? 0, js.SinkKey ?? 0);
    }
}