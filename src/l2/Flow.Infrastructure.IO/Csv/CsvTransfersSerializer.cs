using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Csv;

internal class CsvTransfersSerializer : CsvSerializer
{
    private readonly CsvConfiguration config;

    public CsvTransfersSerializer(CsvConfiguration config) : base(config)
    {
        this.config = config;
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteTransfers(StreamWriter writer, IEnumerable<Transfer> transfers, CancellationToken ct)
    {
        await Write<Transfer, TransferRow, TransferRowMap>(writer, transfers, t => (TransferRow)t, ct);
    }

    [Obsolete("Use Write() instead")]
    public async Task WriteTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, CancellationToken ct)
    {
        await Write(writer, keys, t => (TransferKeyRow)t, ct);
    }

    [Obsolete("Use Read() instead")]
    public async Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader, CancellationToken ct)
    {
        return await Read<TransferKey, TransferKeyRow>(reader, r => (TransferKey)r, ct);
    }
}