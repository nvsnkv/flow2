using CsvHelper.Configuration;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class RecordedTransactionsReader : CsvReader<RecordedTransaction, RecordedTransactionRow>
{
    public RecordedTransactionsReader(CsvConfiguration config) : base(config, r => (RecordedTransaction)r)
    {
    }
}