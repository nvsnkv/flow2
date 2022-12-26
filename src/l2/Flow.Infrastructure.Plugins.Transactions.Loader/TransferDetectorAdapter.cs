using Flow.Application.Transactions.Transfers;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Plugins.Transactions.Contract;

namespace Flow.Infrastructure.Plugins.Transactions.Loader;

internal sealed class TransferDetectorAdapter : ITransferDetector
{
    private readonly ITransferDetectionPlugin plugin;

    public TransferDetectorAdapter(ITransferDetectionPlugin plugin)
    {
        this.plugin = plugin;
    }

    public bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right) => plugin.CheckIsTransfer(left, right);

    public Task<Transfer> Create(RecordedTransaction left, RecordedTransaction right, CancellationToken ct) => plugin.Create(left, right, ct);

    public DetectionAccuracy Accuracy => plugin.Accuracy;
}