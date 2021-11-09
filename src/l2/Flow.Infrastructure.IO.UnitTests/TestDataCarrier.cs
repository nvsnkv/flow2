using System;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.UnitTests;

public abstract class TestDataCarrier
{
    private static readonly AccountInfo Account = new("Account", "The Bank");
    private static readonly AccountInfo CyryllicAccount = new("Счёт", "Банк");

    protected static readonly Transaction[] Transactions = new[]
    {
        new Transaction(DateTime.UtcNow.Date, -150, "RUB", null, "local store", Account),
        new Transaction(DateTime.UtcNow.Date, -150.5M, "RUB", "famous", "local store", Account),
        new Transaction(DateTime.UtcNow.Date, -150.5M, "RUB", "Тест", "Кириллицы", CyryllicAccount),
        new Transaction(new(2021, 11, 06, 11, 30, 00), 2, "EUR", null, "'\"Quotes\" test", Account),
        new Transaction(new(2021, 11, 05), 0.333M, "RUR", "Special symbols", "\',.;\" test", Account)
    };

    protected static readonly RecordedTransaction[] RecordedTransactions = {
        new(1, Transactions[0]) { Overrides = new(null, null, null) },
        new(2, Transactions[1]) {Overrides = new(null, null, null) },
        new(3, Transactions[2]) { Overrides = new("#1", "2", "noway") },
        new(4, Transactions[3]) { Overrides = new("Overriden Category", "Overriden Title", null) },
        new(5, Transactions[4]) { Overrides = new(null, null, "Comment on #5") }
    };
}