using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Common;
using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Csv;

internal class CsvRejectionsWriter
{
    private readonly CsvConfiguration config;

    public CsvRejectionsWriter(CsvConfiguration config)
    {
        this.config = config;
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, CancellationToken ct)
    {
        await Write<RejectedTransaction, Transaction, TransactionRow>(
            writer,
            rejections,
            r => r.Transaction is RecordedTransaction rr ? (RecordedTransactionRow)rr : (TransactionRow)r.Transaction,
            ct);
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransferKey> rejections, CancellationToken ct)
    {
        await Write<RejectedTransferKey, TransferKey, TransferKeyRow>(
            writer,
            rejections,
            r => (TransferKeyRow)r.TransferKey,
            ct);
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedRate> rejections, CancellationToken ct)
    {
        await Write<RejectedRate, ExchangeRate, ExchangeRateRow>(
            writer,
            rejections,
            r => (ExchangeRateRow)r.Rate,
            ct);
    }

    private async Task Write<TR, TE, TRow>(StreamWriter writer, IEnumerable<TR> rejections, Func<TR, TRow> convertFunc, CancellationToken ct) where TR : RejectedEntity<TE>
    {
        await using var csvWriter = new CsvWriter(writer, config);
        foreach (var rejected in rejections)
        {
            var record = convertFunc(rejected);
            await csvWriter.WriteRecordsAsync(Enumerable.Repeat(record, 1), ct);

            foreach (var reason in rejected.Reasons)
            {
                await writer.WriteLineAsync(reason.ToCharArray(), ct);
            }
        }
    }
}