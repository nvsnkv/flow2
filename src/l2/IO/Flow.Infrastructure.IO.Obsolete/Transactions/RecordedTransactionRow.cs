using System.Diagnostics.CodeAnalysis;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Transactions;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class RecordedTransactionRow : TransactionRow
{
    public long? KEY { get; init; }

    public static explicit operator RecordedTransaction(RecordedTransactionRow row)
    {
        var (transaction, overrides) = row;
        return new RecordedTransaction(row.KEY ?? default, transaction)
        {
            Overrides = overrides
        };
    }

    public static explicit operator RecordedTransactionRow(RecordedTransaction t)
    {
        return new RecordedTransactionRow
        {
            TIMESTAMP = t.Timestamp.ToLocalTime(),
            AMOUNT = t.Amount,
            CURRENCY = t.Currency,
            CATEGORY = t.Category,
            TITLE = t.Title,
            ACCOUNT = t.Account.Name,
            BANK = t.Account.Bank,
            KEY = t.Key,
            CATEGORY_OVERRIDE = t.Overrides?.Category,
            TITLE_OVERRIDE = t.Overrides?.Title,
            COMMENT = t.Overrides?.Comment
        };
    }
}