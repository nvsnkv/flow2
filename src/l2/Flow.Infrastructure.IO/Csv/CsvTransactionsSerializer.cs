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

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        csvWriter.Context.RegisterClassMap<TransactionRowMap>();

        await csvWriter.WriteRecordsAsync(transactions.Select(t => (TransactionRow)t), ct);
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions,  CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        csvWriter.Context.RegisterClassMap<RecordedTransactionRowMap>();

        await csvWriter.WriteRecordsAsync(transactions.Select(t => (RecordedTransactionRow)t), ct);
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