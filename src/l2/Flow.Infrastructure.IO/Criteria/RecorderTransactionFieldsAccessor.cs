using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Criteria;

internal class RecorderTransactionFieldsAccessor
{
    private readonly RecordedTransaction transaction;

    public RecorderTransactionFieldsAccessor(Transaction transaction)
    {
        this.transaction = transaction is RecordedTransaction rt ? rt : new RecordedTransaction(0, transaction);
    }

    public long Key => transaction.Key;

    public string Comment => transaction.Overrides?.Comment ?? string.Empty;

    public string CategoryOverride => transaction.Overrides?.Category ?? string.Empty;

    public string TitleOverride => transaction.Overrides?.Title ?? string.Empty;


}