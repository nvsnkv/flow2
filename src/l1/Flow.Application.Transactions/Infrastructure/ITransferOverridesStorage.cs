using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Infrastructure;

public interface ITransferOverridesStorage
{
    public Task<IEnumerable<TransferKey>> GetOverrides(CancellationToken ct);

    Task<IEnumerable<RejectedTransferKey>> Enforce(IEnumerable<TransferKey> t, CancellationToken ct);

    Task<IEnumerable<RejectedTransferKey>> Abandon(IEnumerable<TransferKey> t, CancellationToken ct);
}