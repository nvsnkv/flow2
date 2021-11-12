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

    public IEnumerable<Transfer> Build()
    {
        return transactions.Join(transactions,
            t => t.Amount < 0,
            t => t.Amount > 0,
            (l, r) => detectors.FirstOrDefault(d => d.CheckIsTransfer(l, r))?.Create(l, r))
            .Where(t => t != null)!;
    }
}