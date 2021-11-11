using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

internal class TransfersBuilder
{
    private readonly IList<ITransferDetector> detectors = new List<ITransferDetector>();
    private readonly IEnumerable<RecordedTransaction> transactions;

    public TransfersBuilder(IEnumerable<RecordedTransaction> transactions)
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

    public IEnumerable<Transfer> Build()
    {
        var list = transactions.ToList();
        return list.Join(list,
                    t => t,
                    t => t,
                    (l, r) => detectors.FirstOrDefault(d => d.IsTransfer(l, r))?.Create(l, r))
                .Where(t => t != null)!;
    }
}