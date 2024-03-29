﻿using System.Linq.Expressions;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Contract;

public interface IAccountant
{
    Task<IEnumerable<RecordedTransaction>> GetTransactions(Expression<Func<RecordedTransaction, bool>>? conditions, CancellationToken ct);

    Task<IEnumerable<RejectedTransaction>> CreateTransactions(IEnumerable<IncomingTransaction> transactions, CancellationToken ct);

    Task<IEnumerable<RejectedTransaction>> UpdateTransactions(IEnumerable<RecordedTransaction> transactions, CancellationToken ct);

    Task<int> DeleteTransactions(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct);

    Task<IEnumerable<IEnumerable<RecordedTransaction>>> GuessDuplicates(Expression<Func<RecordedTransaction, bool>>? conditions, int daysRange, CancellationToken ct);

    IAsyncEnumerable<Transfer> GetTransfers(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct);

    IAsyncEnumerable<Transfer> GuessTransfers(Expression<Func<RecordedTransaction, bool>> conditions, CancellationToken ct);

    Task<IEnumerable<RejectedTransferKey>> EnforceTransfers(IEnumerable<TransferKey> keys, CancellationToken ct);

    Task<IEnumerable<RejectedTransferKey>> AbandonTransfers(IEnumerable<TransferKey> keys, CancellationToken ct);
}
