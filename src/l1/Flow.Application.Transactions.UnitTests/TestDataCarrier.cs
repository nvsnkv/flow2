using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.UnitTests;

public class TestDataCarrier
{
    private static readonly AccountInfo Account = new AccountInfo("name", "bank");

    protected static readonly IDictionary<long, RecordedTransaction> Transactions = new[]
    {
        new RecordedTransaction(1, DateTime.Now, -10, "EUR", "Transfer 1", "Source", Account),
        new RecordedTransaction(2, DateTime.Now, 10, "EUR", "Transfer 1", "Sink", Account),
        new RecordedTransaction(3, DateTime.Now, 10, "EUR", null, "Regular", Account),
        new RecordedTransaction(4, DateTime.Now, 20, "EUR", "Transfer 2", "Sink", Account),
        new RecordedTransaction(5, DateTime.Now.AddMilliseconds(-5), -20, "EUR", "Transfer 2", "Source", Account),
        new RecordedTransaction(6, DateTime.Now, 20, "EUR", null, "Regular 2", Account),
        new RecordedTransaction(7, DateTime.Now, -30, "RUB", null, "Regular 3", Account),

        new RecordedTransaction(344, DateTime.Now, 20, "RUB", null, "Regular 2", Account),
        new RecordedTransaction(400, DateTime.Now, -30, "RUB", null, "Regular 3", Account),
    }.ToDictionary(r => r.Key);
}