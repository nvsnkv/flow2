using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Transactions;

internal class TransfersWriter
{
    private readonly CsvConfiguration config;

    public TransfersWriter(CsvConfiguration config)
    {
        this.config = config;
    }

    public async Task Write(StreamWriter writer, IAsyncEnumerable<Transfer> transfers, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        await foreach (var transfer in transfers.WithCancellation(ct))
        {
            var summaryRow = (TransferRow)transfer;
            var sourceRow = (RecordedTransactionRow)transfer.Source;
            var sinkRow = (RecordedTransactionRow)transfer.Sink;

            await csvWriter.WriteRecordsAsync(Enumerable.Repeat(summaryRow, 1), ct);
            await csvWriter.WriteRecordsAsync(Enumerable.Repeat(sourceRow, 1), ct);
            await csvWriter.WriteRecordsAsync(Enumerable.Repeat(sinkRow, 1), ct);
        }
    }
}