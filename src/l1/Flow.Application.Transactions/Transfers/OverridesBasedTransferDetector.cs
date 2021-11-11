using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

class OverridesBasedTransferDetector : ITransferDetector
{
    private readonly HashSet<TransferKey> enforced;
    
    public OverridesBasedTransferDetector(HashSet<TransferKey> enforced)
    {
        this.enforced = enforced;
    }

    public bool IsTransfer(RecordedTransaction left, RecordedTransaction right)
    {
        return enforced.Contains(new TransferKey(left.Key, right.Key));
    }

    public Transfer Create(RecordedTransaction left, RecordedTransaction right)
    {
        if (!IsTransfer(left, right))
        {
            throw new InvalidOperationException("Given transactions does not listed in overrides!");
        }

        return new Transfer(left, right)
        {
            Comment = "User-defined transfer"
        };
    }

    public static async Task<ITransferDetector> Create(ITransferOverridesStorage storage, CancellationToken ct)
    {
        var overrides = await storage.GetOverrides(ct);
        return new OverridesBasedTransferDetector(overrides.ToHashSet());
    }
}