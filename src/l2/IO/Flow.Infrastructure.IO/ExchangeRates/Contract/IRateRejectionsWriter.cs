using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.ExchangeRates.Contract;

public interface IRateRejectionsWriter
{
    Task WriteRejections(StreamWriter writer, IEnumerable<RejectedRate> rejections, SupportedFormat format, CancellationToken ct);
}