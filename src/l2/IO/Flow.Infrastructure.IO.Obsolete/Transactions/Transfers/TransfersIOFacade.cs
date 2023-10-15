using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Generics;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Infrastructure.IO.Transactions.Transfers;

internal class TransfersIOFacade : ITransferKeysReader, ITransfersWriter
{
    private readonly CsvSerializer csv;
    private readonly JsonSerializer json;
    private readonly TransfersWriter transfersWriter;

    public TransfersIOFacade(CsvSerializer csv, JsonSerializer json, TransfersWriter transfersWriter)
    {
        this.csv = csv;
        this.json = json;
        this.transfersWriter = transfersWriter;
    }

    public async Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader, OldSupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            OldSupportedFormat.CSV => await csv.Read<TransferKey, TransferKeyRow>(reader, r => (TransferKey)r, ct),
            OldSupportedFormat.JSON => await json.Read(reader, (JsonTransferKey j) => (TransferKey)j),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task WriteTransfers(StreamWriter writer, IAsyncEnumerable<Transfer> transfers, OldSupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case OldSupportedFormat.CSV:
                await transfersWriter.Write(writer, transfers, ct);
                return;

            case OldSupportedFormat.JSON:
                await json.Write(writer, transfers, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }

    public async Task WriteTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, OldSupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case OldSupportedFormat.CSV:
                await csv.Write(writer, keys, t => (TransferKeyRow)t, ct);
                return;

            case OldSupportedFormat.JSON:
                await json.Write(writer, keys, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}