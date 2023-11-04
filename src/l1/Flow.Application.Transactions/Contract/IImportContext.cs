using System.Linq.Expressions;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.Contract;

public interface IImportContext : IAsyncDisposable
{
    IEnumerable<long> RecordedTransactionKeys { get; }

    int RecordedTransactionsCount { get; }

    Expression<Func<RecordedTransaction, bool>> Criteria { get; }
}
