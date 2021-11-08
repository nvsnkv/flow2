using Flow.Domain.Transactions;
using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.Json;

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
        return new Transaction(js.Timestamp ?? default(DateTime), js.Amount ?? default(decimal), js.Currency ?? string.Empty, js.Category, js.Title ?? string.Empty, acc);
    }
}