using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.Plugins.Transactions.Contract;

public interface ITransferDetectionPlugin : IPlugin
{
    bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right);

    Task<Transfer> Create(RecordedTransaction left, RecordedTransaction right, CancellationToken ct);

    DetectionAccuracy Accuracy { get; }
}