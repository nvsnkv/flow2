using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class TransactionsIOFacade : ITransactionsReader, ITransactionsWriter, IRejectionsWriter, ITransferKeysReader, ITransfersWriter
{
    private readonly CsvTransactionsSerializer csv;
    private readonly JsonTransactionsSerializer json;

    public TransactionsIOFacade(CsvTransactionsSerializer csv, JsonTransactionsSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.ReadTransactions(reader, ct),
            SupportedFormat.JSON => await json.ReadTransactions(reader),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.ReadRecordedTransactions(reader, ct),
            SupportedFormat.JSON => await json.ReadRecordedTransactions(reader),
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
                await json.WriteTransactions(writer, transactions, ct);
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
                await json.WriteRecordedTransactions(writer, transactions, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.WriteRejections(writer, rejections, ct);
                return;

            case SupportedFormat.JSON:
                await json.WriteRejections(writer, rejections, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransferKey> rejections, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.WriteRejections(writer, rejections, ct);
                return;

            case SupportedFormat.JSON:
                await json.WriteRejections(writer, rejections, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.ReadTransferKeys(reader, ct),
            SupportedFormat.JSON => await json.ReadTransferKeys(reader),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task WriteTransfers(StreamWriter writer, IEnumerable<Transfer> transfers, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.WriteTransfers(writer, transfers, ct);
                return;

            case SupportedFormat.JSON:
                await json.WriteTransfers(writer, transfers, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task WriteTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.WriteTransferKeys(writer, keys, ct);
                return;

            case SupportedFormat.JSON:
                await json.WriteTransferKeys(writer, keys, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}