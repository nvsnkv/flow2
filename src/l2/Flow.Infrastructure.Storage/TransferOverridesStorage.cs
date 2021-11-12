using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.Storage;

internal class TransferOverridesStorage : ITransferOverridesStorage
{
    public Task<IEnumerable<TransferKey>> GetOverrides(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RejectedTransferKey>> Enforce(IEnumerable<TransferKey> t, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RejectedTransferKey>> Abandon(IEnumerable<TransferKey> t, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}