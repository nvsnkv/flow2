using Flow.Domain.Transactions;

namespace Flow.Plugins.Transactions.FlexibleTransactionsReader.Impl;

internal record InputDataRow(
    DateTime? Timestamp,
    decimal? Amount,
    string? Currency,
    string? Category,
    string? Title,
    string? Account,
    string? Bank,
    string? Comment,
    string? TitleOverride,
    string? CategoryOverride
)
{
    public InputDataRow() : this(null,null,null,null,null,null,null,null,null,null) {}

    public void Deconstruct(out Transaction t, out Overrides? overrides)
    {
        var account = new AccountInfo(Account ?? string.Empty, Bank ?? string.Empty);
         overrides = new Overrides(CategoryOverride, TitleOverride, Comment);

         t = new Transaction(Timestamp ?? DateTime.MinValue, Amount ?? 0, Currency, Category, Title, account);
    }
};
