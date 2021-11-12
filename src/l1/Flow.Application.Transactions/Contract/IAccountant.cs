using System.Linq.Expressions;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.Contract;

public interface IAccountant
{
    Task<IEnumerable<RecordedTransaction>> GetTransactions(Expression<Func<RecordedTransaction, bool>>? conditions, CancellationToken ct);

    Task<IEnumerable<RejectedTransaction>> CreateTransactions(IEnumerable<Transaction> transactions, CancellationToken ct);

    Task<IEnumerable<RejectedTransaction>> UpdateTransactions(IEnumerable<RecordedTransaction> transactions, CancellationToken ct);

    Task<int> DeleteTransactions(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct);
}