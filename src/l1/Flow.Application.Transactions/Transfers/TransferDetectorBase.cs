using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Transfers;

internal abstract class TransferDetectorBase : ITransferDetector
{
    private readonly string comment;

    protected TransferDetectorBase(string comment)
    {
        this.comment = comment;
    }

    public abstract bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right);

    public Transfer Create(RecordedTransaction left, RecordedTransaction right)
    {
        if (!CheckIsTransfer(left, right))
        {
            throw new InvalidOperationException("Given transactions does not belong to a single transfer!");
        }

        return new Transfer(left, right)
        {
            Comment = comment
        };
    }
}