using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Csv;

internal class CsvTransactionsSerializer : CsvSerializer
{
    private readonly CsvConfiguration config;

    public CsvTransactionsSerializer(CsvConfiguration config) : base(config)
    {
        this.config = config;
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await Write<Transaction, TransactionRow, TransactionRowMap>(writer, transactions, t => (TransactionRow)t, ct);
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions,  CancellationToken ct)
    {
        await Write<RecordedTransaction, RecordedTransactionRow, RecordedTransactionRowMap>(writer, transactions, t => (RecordedTransactionRow)t, ct);
    }

    [Obsolete("Use Read() instead")]
    public async Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, CancellationToken ct)
    {
        return await Read(reader, (TransactionRow r) => (Transaction)r, ct);
    }

    [Obsolete("Use Read() instead")]
    public async Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, CancellationToken ct)
    {
        return await Read(reader, (RecordedTransactionRow r) => (RecordedTransaction)r, ct);
    }
}