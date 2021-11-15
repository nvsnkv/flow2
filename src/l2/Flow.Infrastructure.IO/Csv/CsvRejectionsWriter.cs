using CsvHelper;
using CsvHelper.Configuration;
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

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        foreach (var rejected in rejections)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            csvWriter.WriteRecord(rejected.Transaction is RecordedTransaction rr ? (RecordedTransactionRow)rr : (TransactionRow)rejected.Transaction);
            
            foreach (var reason in rejected.Reasons)
            {
                await writer.WriteLineAsync(reason.ToCharArray(), ct);
            }
        }
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransferKey> rejections, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        foreach (var rejected in rejections)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            csvWriter.WriteRecord((TransferKeyRow)rejected.TransferKey);

            foreach (var reason in rejected.Reasons)
            {
                await writer.WriteLineAsync(reason.ToCharArray(), ct);
            }
        }
    }
}