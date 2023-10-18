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
    public static explicit operator (Transaction, Overrides)(InputDataRow input)
    {
        var account = new AccountInfo(input.Account ?? string.Empty, input.Bank ?? string.Empty);
        var overrides = new Overrides(input.CategoryOverride, input.TitleOverride, input.Comment);

        var transaction = new Transaction(input.Timestamp ?? DateTime.MinValue, input.Amount ?? 0, input.Currency, input.Category, input.Title, account);

        return (transaction, overrides);
    }
};
