using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Transactions.Contract;

public interface IRejectionsWriter
{
    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, OldSupportedFormat format, CancellationToken ct);

    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransferKey> rejections, OldSupportedFormat format, CancellationToken ct);
}
