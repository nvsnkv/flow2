using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

internal class OverridesBasedTransferDetector : TransferDetectorBase
{
    private readonly HashSet<TransferKey> enforced;

    private OverridesBasedTransferDetector(HashSet<TransferKey> enforced):base("User defined transfer")
    {
        this.enforced = enforced;
    }

    public override bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right)
    {
        return enforced.Contains(new TransferKey(left.Key, right.Key));
    }
    
    public static async Task<ITransferDetector> Create(ITransferOverridesStorage storage, CancellationToken ct)
    {
        var overrides = await storage.GetOverrides(ct);
        return new OverridesBasedTransferDetector(overrides.ToHashSet());
    }
}