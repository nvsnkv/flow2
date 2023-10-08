using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class RejectionsWriter : IRejectionsWriter
{
    private readonly CsvRejectionsWriter csv;
    private readonly JsonRejectionsWriter json;

    public RejectionsWriter(CsvRejectionsWriter csv, JsonRejectionsWriter json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.Write<RejectedTransaction, Transaction, TransactionRow>(
                    writer,
                    rejections,
                    r => r.Transaction is RecordedTransaction rr ? (RecordedTransactionRow)rr : (TransactionRow)r.Transaction,
                    ct);
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
                await csv.Write<RejectedTransferKey, TransferKey, TransferKeyRow>(
                    writer,
                    rejections,
                    r => (TransferKeyRow)r.TransferKey,
                    ct);
                return;

            case SupportedFormat.JSON:
                await json.WriteRejections(writer, rejections, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedRate> rejections, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.Write<RejectedRate, ExchangeRate, ExchangeRateRow>(
                    writer,
                    rejections,
                    r => (ExchangeRateRow)r.Rate,
                    ct);
                return;

            case SupportedFormat.JSON:
                await json.WriteRejections(writer, rejections, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}