using CsvHelper.Configuration;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class RecordedTransactionsWriter : CsvWriter<RecordedTransaction, RecordedTransactionRow, RecordedTransactionRowMap>
{
    public RecordedTransactionsWriter(CsvConfiguration config) : base(config, r => (RecordedTransactionRow)r)
    {
    }
}