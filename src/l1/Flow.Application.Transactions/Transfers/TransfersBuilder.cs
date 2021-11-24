using Flow.Domain.Common;
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

    public IEnumerable<Transfer> Build(CancellationToken ct)
    {
        var sources = transactions.OrderBy(t => t.Timestamp);
        var sinks = transactions.OrderBy(t => t.Timestamp).ToList();
        var usedSinks = new HashSet<int>();

        var result = sources.Select(async source =>
            {
                for (var i = 0; i < sinks.Count; i++)
                {
                    if (!usedSinks.Contains(i))
                    {
                        var sink = sinks[i];

                        var detector = detectors.FirstOrDefault(d => d.CheckIsTransfer(source, sink));
                        if (detector != null)
                        {
                            var transfer = await detector.Create(source, sink, ct);
                            usedSinks.Add(i);
                            return transfer;
                        }
                    }
                }

                return null;
            })
            .Select(t => t.Await(ct))
            .Where(t => t != null)
            .Cast<Transfer>();

        return result;
    }
}