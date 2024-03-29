﻿using System.Linq.Expressions;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.Infrastructure;

public interface ITransactionsStorage
{
    public Task<IEnumerable<RejectedTransaction>> Create(IEnumerable<IncomingTransaction> transactions, CancellationToken ct);

    public Task<IEnumerable<RecordedTransaction>> Read(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct);

    public Task<IEnumerable<RejectedTransaction>> Update(IEnumerable<RecordedTransaction> transactions, CancellationToken ct);

    public Task<int> Delete(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct);
}
