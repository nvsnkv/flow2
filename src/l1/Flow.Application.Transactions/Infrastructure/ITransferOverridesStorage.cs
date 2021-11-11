using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Infrastructure;

public interface ITransferOverridesStorage
{
    public Task<IEnumerable<TransferKey>> GetOverrides(CancellationToken ct);

    Task Enforce(TransferKey t, CancellationToken ct);

    Task Abandon(TransferKey t, CancellationToken ct);
}