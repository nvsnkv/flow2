using System.Diagnostics.CodeAnalysis;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Transactions;

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

    public string? COMMENT { get; init; }

    public string? CATEGORY_OVERRIDE { get; init; }

    public string? TITLE_OVERRIDE { get; init; }

    public void Deconstruct(out Transaction t, out Overrides? overrides)
    {
        t = (Transaction)this;
        overrides = null;

        if (!string.IsNullOrEmpty(COMMENT) || !string.IsNullOrEmpty(CATEGORY_OVERRIDE) || !string.IsNullOrEmpty(TITLE_OVERRIDE))
        {
            overrides = new Overrides(CATEGORY_OVERRIDE, TITLE_OVERRIDE, COMMENT);
        }
    }

    public static explicit operator Transaction(TransactionRow row)
    {
        return new Transaction((row.TIMESTAMP ?? default).ToUniversalTime(), row.AMOUNT ?? default, row.CURRENCY ?? string.Empty, row.CATEGORY, row.TITLE ?? string.Empty, new AccountInfo(row.ACCOUNT ?? string.Empty, row.BANK ?? string.Empty));
    }

    public static explicit operator TransactionRow(Transaction t)
    {
        return new TransactionRow
        {
            TIMESTAMP = t.Timestamp.ToLocalTime(),
            AMOUNT = t.Amount,
            CURRENCY = t.Currency,
            CATEGORY = t.Category,
            TITLE = t.Title,
            ACCOUNT = t.Account.Name,
            BANK = t.Account.Bank
        };
    }
}