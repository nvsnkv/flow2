namespace Flow.Application.Transactions.Contract.Events;

public sealed class TransactionRecordedEventArgs : EventArgs
{
    public TransactionRecordedEventArgs(long key)
    {
        Key = key;
    }

    public long Key { get; }
}
