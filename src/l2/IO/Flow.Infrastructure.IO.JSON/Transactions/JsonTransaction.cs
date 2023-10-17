using Flow.Domain.Transactions;
using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.JSON.Transactions;

internal class JsonTransaction
{
    public DateTime? Timestamp { get; set; }

    public decimal? Amount { get; set; }

    public string? Currency { get; set; }

    public string? Category { get; set; }

    public string? Title { get; set; }

    public JsonAccountInfo? Account { get; set; }

    [UsedImplicitly]
    public class JsonAccountInfo
    {
        public string? Name { get; set; }

        public string? Bank { get; set; }
    }

    public static explicit operator Transaction(JsonTransaction js)
    {
        var acc = new AccountInfo(js.Account?.Name ?? string.Empty, js.Account?.Bank ?? string.Empty);
        return new Transaction(js.Timestamp ?? default, js.Amount ?? default, js.Currency ?? string.Empty, js.Category, js.Title ?? string.Empty, acc);
    }

    public static explicit operator JsonTransaction(Transaction t)
    {
        return new JsonTransaction
        {
            Account = new JsonAccountInfo
            {
                Name = t.Account.Name,
                Bank = t.Account.Bank
            },

            Amount = t.Amount,
            Category = t.Category,
            Currency = t.Currency,
            Title = t.Title,
            Timestamp = t.Timestamp
        };
    }
}
