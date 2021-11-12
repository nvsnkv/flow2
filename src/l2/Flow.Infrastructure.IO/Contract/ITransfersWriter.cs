using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface ITransfersWriter
{
    Task WriterTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, SupportedFormat format, CancellationToken ct);

    Task WriterTransfers(StreamWriter writer, IEnumerable<Transfer> transfers, SupportedFormat format, CancellationToken ct);
}