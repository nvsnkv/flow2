using System.Linq.Expressions;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis.Contract;

public interface IAnalyst
{
    IAsyncEnumerable<FlowItem> GetFlow(Expression<Func<RecordedTransaction, bool>> conditions, string? targetCurrency, CancellationToken ct);
}