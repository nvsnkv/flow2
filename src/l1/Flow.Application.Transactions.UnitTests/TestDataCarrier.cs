using System;
using System.Collections.Generic;
using System.Linq;
using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.UnitTests;

public class TestDataCarrier
{
    private static readonly AccountInfo Account = new("name", "bank");

    protected static readonly IDictionary<long, RecordedTransaction> Transactions = new[]
    {
        new RecordedTransaction(1, DateTime.Now, -10, "EUR", "Transfer 1", "Source", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(2, DateTime.Now, 10, "EUR", "Transfer 1", "Sink", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(3, DateTime.Now, 10, "EUR", null, "Regular", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(4, DateTime.Now, 20, "EUR", "Transfer 2", "Sink", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(5, DateTime.Now.AddMilliseconds(-5), -20, "EUR", "Transfer 2", "Source", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(6, DateTime.Now, 20, "EUR", null, "Regular 2", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(7, DateTime.Now, -30, "RUB", null, "Regular 3", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(8, DateTime.Now, -30, "EUR", null, "Regular 3", Account, Guid.NewGuid().ToString()),
        
        new RecordedTransaction(344, DateTime.Now, 20, "RUB", null, "Regular 2", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(400, DateTime.Now, -30, "RUB", null, "Regular 3", Account, Guid.NewGuid().ToString()),

        new RecordedTransaction(500, DateTime.Now, -100, "RUB", "Transfer", "With Conversion", Account, Guid.NewGuid().ToString()),
        new RecordedTransaction(501, DateTime.Now, 3, "BYN", "Transfer", "With Conversion", Account, Guid.NewGuid().ToString())
    }.ToDictionary(r => r.Key);
}
