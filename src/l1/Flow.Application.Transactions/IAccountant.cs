using System.Linq.Expressions;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions;

public interface IAccountant
{
    Task<IEnumerable<RecordedTransaction>> Get(Expression<Func<Transaction, bool>>? conditions,
        CancellationToken ct);

    Task<IEnumerable<RejectedTransaction>> Create(IEnumerable<Transaction> transactions, CancellationToken ct);
    Task<IEnumerable<RejectedTransaction>> Update(IEnumerable<RecordedTransaction> transactions, CancellationToken ct);
    Task<int> Delete(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct);
}