namespace Flow.Application.Transactions.Transfers;

internal class DetectorsCollection : List<ITransferDetector>
{
    public DetectorsCollection()
    {
    }

    public DetectorsCollection(IEnumerable<ITransferDetector> collection) : base(collection)
    {
    }

    public DetectorsCollection(int capacity) : base(capacity)
    {
    }
}