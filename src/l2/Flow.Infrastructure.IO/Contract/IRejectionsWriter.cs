using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface IRejectionsWriter
{
    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, SupportedFormat format, CancellationToken ct);

    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransferKey> rejections, SupportedFormat format, CancellationToken ct);

    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedRate> rejections, SupportedFormat format, CancellationToken ct);
}