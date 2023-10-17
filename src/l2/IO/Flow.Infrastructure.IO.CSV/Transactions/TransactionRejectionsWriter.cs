using CsvHelper.Configuration;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.CSV.Transactions;

internal class TransactionRejectionsWriter : CsvRejectionsWriter<RejectedTransaction, Transaction, TransactionRow>
{
    public TransactionRejectionsWriter(CsvConfiguration config) : base(
        config, r => r.Transaction is RecordedTransaction rr ? (RecordedTransactionRow)rr : (TransactionRow)r.Transaction
    )
    {
    }
}
