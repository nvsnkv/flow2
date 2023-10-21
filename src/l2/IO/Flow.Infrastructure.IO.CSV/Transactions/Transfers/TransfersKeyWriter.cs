using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.CSV.Transactions.Transfers;

internal class TransfersKeyWriter : CsvWriter<TransferKey, TransferKeyRow, TransferRowMap>, IFormatSpecificWriter<Transfer>
{
    private readonly CsvConfiguration config;

    public TransfersKeyWriter(CsvConfiguration config) : base(config, k => (TransferKeyRow)k)
    {
        this.config = config;
    }

    public async Task Write(StreamWriter writer, IEnumerable<Transfer> items, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        foreach (var transfer in items)
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
