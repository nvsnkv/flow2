using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Criteria.UnitTests;

public abstract class TestDataCarrier
{
    private static readonly AccountInfo Account = new("Account", "The Bank");
    private static readonly AccountInfo CyrillicAccount = new("Счёт", "Банк");

    protected static readonly Transaction[] Transactions = {
        new(DateTime.UtcNow.Date, -150, "RUB", null, "local store", Account),
        new(DateTime.UtcNow.Date, -150.5M, "RUB", "famous", "local store", Account),
        new(DateTime.UtcNow.Date, -150.5M, "RUB", "Тест", "Кириллицы", CyrillicAccount),
        new(new(2021, 11, 06, 11, 30, 00), 2, "EUR", null, "'\"Quotes\" test", Account),
        new(new(2021, 11, 05), 0.333M, "RUR", "Special symbols", "\',.;\" test", Account)
    };

    protected static readonly RecordedTransaction[] RecordedTransactions =
    {
        new(1, Transactions[0], new object().GetHashCode().ToString()) { Overrides = new(null, null, null) },
        new(2, Transactions[1], new object().GetHashCode().ToString()) { Overrides = new(null, null, null) },
        new(3, Transactions[2], new object().GetHashCode().ToString()) { Overrides = new("#1", "2", "noway") },
        new(4, Transactions[3], new object().GetHashCode().ToString()) { Overrides = new("Overriden Category", "Overriden Title", null) },
        new(5, Transactions[4], new object().GetHashCode().ToString()) { Overrides = new(null, null, "Comment on #5") }
    };

    protected static readonly TransferKey[] TransferKeys =
    {
        new(1, 2),
        new(3, 5),
        new(1000, 3000)
    };

    protected readonly ExchangeRate[] Rates =
    {
        new("RUB", "BYN", DateTime.Parse("2021-11-16"), 0.0034M),
        new("RUB", "BYN", DateTime.Parse("2021-11-15"), 0.0033M),
        new("RUB", "EUR", DateTime.Parse("2021-11-16"), 82.5244M),
        new("EUR", "RUB", DateTime.Parse("2021-11-16"), 0.0121M)
    };

}
