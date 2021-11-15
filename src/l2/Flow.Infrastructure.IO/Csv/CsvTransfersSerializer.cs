using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Csv;

internal class CsvTransfersSerializer
{
    private readonly CsvConfiguration config;

    public CsvTransfersSerializer(CsvConfiguration config)
    {
        this.config = config;
    }

    public async Task WriteTransfers(StreamWriter writer, IEnumerable<Transfer> transfers, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        csvWriter.Context.RegisterClassMap<TransferRowMap>();

        await csvWriter.WriteRecordsAsync(transfers.Select(k => (TransferRow)k), ct);
    }

    public async Task WriteTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        await csvWriter.WriteRecordsAsync(keys.Select(k => (TransferKeyRow)k), ct);
    }

    public async Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, config);
        var result = new List<TransferKey>();

        await foreach (var row in csvReader.GetRecordsAsync(typeof(TransferKeyRow), ct))
        {
            result.Add((TransferKey)(TransferKeyRow)row);
        }

        return result;
    }
}