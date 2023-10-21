using System.Diagnostics.CodeAnalysis;
using Flow.Application.Transactions.Contract;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Plugins.Contract;

namespace Flow.Plugins.Transactions.TKF;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public sealed class Bootstrapper : IPluginsBootstrapper
{
    public IEnumerable<IPlugin> GetPlugins()
    {
        yield return new TransfersDetector();
    }
}

public sealed class TransfersDetector : ITransferDetector, IPlugin
{
    public bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right)
    {
        return left.Timestamp == right.Timestamp
               && left.Amount < 0
               && right.Amount + left.Amount == 0
               && left.Category == "Переводы/иб"
               && left.Title == "Перевод между счетами"
               && right.Category == "Другое"
               && right.Title == "Перевод между счетами";
    }

    public Task<Transfer> Create(RecordedTransaction left, RecordedTransaction right, CancellationToken ct)
    {
        var result = new Transfer(left, right, DetectionAccuracy.Exact)
        {
            Comment = "Перевод между счетами ТКФ"
        };

        return Task.FromResult(result);
    }

    public DetectionAccuracy Accuracy => DetectionAccuracy.Exact;
}
