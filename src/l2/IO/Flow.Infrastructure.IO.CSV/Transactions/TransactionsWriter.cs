using CsvHelper.Configuration;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class TransactionsWriter : CsvWriter<Transaction, TransactionRow, TransactionRowMap>
{
    public TransactionsWriter(CsvConfiguration config) : base(config, t => (TransactionRow)t)
    {
    }
}