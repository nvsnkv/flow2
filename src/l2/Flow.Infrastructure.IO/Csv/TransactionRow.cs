using System.Diagnostics.CodeAnalysis;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Csv;

[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class TransactionRow
{
    public DateTime? TIMESTAMP { get; init; }

    public decimal? AMOUNT { get; init; }

    public string? CURRENCY { get; init; }

    public string? CATEGORY { get; init; }

    public string? TITLE { get; init; }

    public string? ACCOUNT { get; init; }

    public string? BANK { get; init; }

    public static explicit operator Transaction(TransactionRow row)
    {
        return new Transaction(row.TIMESTAMP ?? default, row.AMOUNT ?? default, row.CURRENCY ?? string.Empty, row.CATEGORY, row.TITLE ?? string.Empty, new AccountInfo(row.ACCOUNT ?? string.Empty, row.BANK ?? string.Empty));
    }

    public static explicit operator TransactionRow(Transaction t)
    {
        return new TransactionRow
        {
            TIMESTAMP = t.Timestamp,
            AMOUNT = t.Amount,
            CURRENCY = t.Currency,
            CATEGORY = t.Category,
            TITLE = t.Title,
            ACCOUNT = t.Account.Name,
            BANK = t.Account.Bank
        };
    }
}