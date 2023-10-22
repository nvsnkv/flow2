using System.Linq.Expressions;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.Contract;

public interface IImportContext : IAsyncDisposable
{
    int RecordedTransactionsCount { get; }

    Expression<Func<RecordedTransaction, bool>> ImportedTransactionsCriteria { get; }

    Task Complete(CancellationToken ct);

    Task Abort(CancellationToken ct);

}
