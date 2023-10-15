using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Generics;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Infrastructure.IO.Transactions;

internal class DefaultCsvTransactionsSerializer : ITransactionsReader, ITransactionsWriter
{
    private readonly CsvSerializer csv;

    public DefaultCsvTransactionsSerializer(CsvSerializer csv)
    {
        this.csv = csv;
    }

    public SupportedFormat Format => SupportedFormat.CSV;

    public Task<IEnumerable<(Transaction, Overrides?)>> ReadTransactions(StreamReader reader, CancellationToken ct)
    {
        return csv.Read(reader, (TransactionRow r) =>
        {
            var (transaction, overrides) = r;
            return (transaction, overrides);
        }, ct);
    }

    public Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, CancellationToken ct)
    {
        return csv.Read(reader, (RecordedTransactionRow r) => (RecordedTransaction)r, ct);
    }

    public Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        return csv.Write<Transaction, TransactionRow, TransactionRowMap>(writer, transactions, t => (TransactionRow)t, ct);
    }

    public Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        return csv.Write<RecordedTransaction, RecordedTransactionRow, RecordedTransactionRowMap>(writer, transactions, t => (RecordedTransactionRow)t, ct);
    }
}
