using CsvHelper.Configuration;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class RejectedTransactionsWriter : CsvRejectionsWriter<RejectedTransaction, Transaction, TransactionRow>
{
    public RejectedTransactionsWriter(CsvConfiguration config) : base(
        config, r => r.Transaction is RecordedTransaction rr ? (RecordedTransactionRow)rr : (TransactionRow)r.Transaction
    )
    {
    }
}
