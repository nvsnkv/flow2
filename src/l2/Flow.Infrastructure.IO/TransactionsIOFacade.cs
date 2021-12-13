using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class TransactionsIOFacade : ITransactionsReader, ITransactionsWriter
{
    private readonly CsvSerializer csv;
    private readonly JsonSerializer json;

    public TransactionsIOFacade(CsvSerializer csv, JsonSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task<IEnumerable<(Transaction, Overrides?)>> ReadTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.Read(reader, (TransactionRow r) =>
            {
                var (transaction, overrides) = r;
                return (transaction, overrides);
            }, ct),
            SupportedFormat.JSON => await json.Read(reader, (JsonTransaction j) => ((Transaction)j, (Overrides?)null)),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.Read(reader, (RecordedTransactionRow r) => (RecordedTransaction)r, ct),
            SupportedFormat.JSON => await json.Read(reader, (JsonRecordedTransaction j) => (RecordedTransaction)j),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                 await csv.Write<Transaction, TransactionRow, TransactionRowMap>(writer, transactions, t => (TransactionRow)t, ct);
                 return;
                
            case SupportedFormat.JSON:
                await json.Write(writer, transactions, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.Write<RecordedTransaction, RecordedTransactionRow, RecordedTransactionRowMap>(writer, transactions, t => (RecordedTransactionRow)t, ct);
                return;

            case SupportedFormat.JSON:
                await json.Write(writer, transactions, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}