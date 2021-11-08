using System.Diagnostics.CodeAnalysis;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Csv;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class RecordedTransactionRow : TransactionRow
{
    public long? KEY { get; init; }

    public string? COMMENT { get; init; }

    public string? CATEGORY_OVERRIDE { get; init; }

    public string? TITLE_OVERRIDE { get; init; }

    public static explicit operator RecordedTransaction(RecordedTransactionRow row)
    {
        var transaction = (Transaction)(TransactionRow)row;
        var result = new RecordedTransaction(row.KEY ?? default, transaction);

        if (!string.IsNullOrEmpty(row.COMMENT) || !string.IsNullOrEmpty(row.CATEGORY_OVERRIDE) || !string.IsNullOrEmpty(row.TITLE_OVERRIDE))
        {
            result.Overrides = new Overrides(row.CATEGORY_OVERRIDE, row.TITLE_OVERRIDE, row.COMMENT);
        }

        return result;
    }

    public static explicit operator RecordedTransactionRow(RecordedTransaction t)
    {
        return new RecordedTransactionRow()
        {
            TIMESTAMP = t.Timestamp,
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