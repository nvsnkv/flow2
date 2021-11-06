using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO;

internal class TransactionsIOFacade : ITransactionsReader, ITransactionsWriter, IRejectionsWriter
{
    private readonly CsvTransactionsSerializer csv;
    private readonly JsonTransactionsSerializer json;

    public TransactionsIOFacade(CsvTransactionsSerializer csv, JsonTransactionsSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, SupportedFormats format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormats.CSV => await csv.ReadTransactions(reader, ct),
            SupportedFormats.JSON => await json.ReadTransactions(reader),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, SupportedFormats format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormats.CSV => await csv.ReadRecordedTransactions(reader, ct),
            SupportedFormats.JSON => await json.ReadRecordedTransactions(reader),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, SupportedFormats format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormats.CSV:
                 await csv.WriteTransactions(writer, transactions, ct);
                 return;
                
            case SupportedFormats.JSON:
                await json.WriteTransactions(writer, transactions, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, SupportedFormats format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormats.CSV:
                await csv.WriteRecordedTransactions(writer, transactions, ct);
                return;

            case SupportedFormats.JSON:
                await json.WriteRecordedTransactions(writer, transactions, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, SupportedFormats format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormats.CSV:
                await csv.WriteRejections(writer, rejections, ct);
                return;

            case SupportedFormats.JSON:
                await json.WriteRejections(writer, rejections, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}