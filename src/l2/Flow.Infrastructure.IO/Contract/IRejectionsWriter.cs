using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface IRejectionsWriter
{
    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, SupportedFormat format, CancellationToken ct);
}