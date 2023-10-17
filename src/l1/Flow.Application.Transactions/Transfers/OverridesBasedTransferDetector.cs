using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Application.Transactions.Infrastructure;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

internal class OverridesBasedTransferDetector : TransferDetectorBase
{
    private readonly HashSet<TransferKey> enforced;
    
    private OverridesBasedTransferDetector(HashSet<TransferKey> enforced, IExchangeRatesProvider provider):base("User defined transfer", provider, DetectionAccuracy.Exact)
    {
        this.enforced = enforced;
    }

    public override bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right)
    {
        return enforced.Contains(new TransferKey(left.Key, right.Key));
    }
    
    public static async Task<ITransferDetector> Create(ITransferOverridesStorage storage, IExchangeRatesProvider provider, CancellationToken ct)
    {
        var overrides = await storage.GetOverrides(ct);
        return new OverridesBasedTransferDetector(overrides.ToHashSet(), provider);
    }
}
