using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Json;

internal class JsonTransferKey
{
    public long? Source { get; set; }

    public long? Sink { get; set; }

    public static explicit operator TransferKey(JsonTransferKey js)
    {
        return new TransferKey(js.Source ?? 0, js.Sink ?? 0);
    }
}