namespace Flow.Application.Transactions.Contract.Events;

public interface INotifyTransactionRecorded
{
    event TransactionRecordedEventHandler TransactionRecorded;
}
