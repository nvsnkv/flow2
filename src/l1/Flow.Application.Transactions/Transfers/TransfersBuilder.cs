using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

internal class TransfersBuilder
{
    private readonly IList<ITransferDetector> detectors = new List<ITransferDetector>();
    private readonly ICollection<RecordedTransaction> transactions;

    public TransfersBuilder(ICollection<RecordedTransaction> transactions)
    {
        this.transactions = transactions;
    }

    public TransfersBuilder With(ITransferDetector detector)
    {
        if (!detectors.Contains(detector))
        {
            detectors.Add(detector);
        }

        return this;
    }

    public async Task<IEnumerable<Transfer>> Build(CancellationToken ct)
    {
        var tasks = transactions.OrderBy(t => t.Timestamp).Join(transactions.OrderBy(t => t.Timestamp),
                t => t.Amount < 0,
                t => t.Amount > 0,
                (l, r) => detectors.FirstOrDefault(d => d.CheckIsTransfer(l, r))?.Create(l, r, ct))
            .Where(t => t != null);

        var result = new List<Transfer>();
        foreach (var task in tasks)
        {
            result.Add(await (task!));
        }

        return result.DistinctBy(t => t.Source).DistinctBy(t => t.Sink);
    }
}