using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Transactions.Contract;

public interface ITransfersWriter
{
    Task WriteTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, OldSupportedFormat format, CancellationToken ct);

    Task WriteTransfers(StreamWriter writer, IAsyncEnumerable<Transfer> transfers, OldSupportedFormat format, CancellationToken ct);
}