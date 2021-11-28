using Flow.Domain.Analysis;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface IFlowWriter 
{
    Task Write(StreamWriter writer, IAsyncEnumerable<FlowItem> items, SupportedFormat format, CancellationToken ct);
}