using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class TransactionsIOFacade : ITransactionsReader, ITransactionsWriter
{
    private readonly CsvTransactionsSerializer csv;
    private readonly JsonSerializer json;

    public TransactionsIOFacade(CsvTransactionsSerializer csv, JsonSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.ReadTransactions(reader, ct),
            SupportedFormat.JSON => await json.Read(reader, (JsonTransaction j) => (Transaction)j),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.ReadRecordedTransactions(reader, ct),
            SupportedFormat.JSON => await json.Read(reader, (JsonRecordedTransaction j) => (RecordedTransaction)j),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                 await csv.WriteTransactions(writer, transactions, ct);
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
                await csv.WriteRecordedTransactions(writer, transactions, ct);
                return;

            case SupportedFormat.JSON:
                await json.Write(writer, transactions, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}