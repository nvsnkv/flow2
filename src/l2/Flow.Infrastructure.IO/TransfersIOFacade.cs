using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class TransfersIOFacade : ITransferKeysReader, ITransfersWriter
{
    private readonly CsvSerializer csv;
    private readonly JsonSerializer json;

    public TransfersIOFacade(CsvSerializer csv, JsonSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.Read<TransferKey, TransferKeyRow>(reader, r => (TransferKey)r, ct),
            SupportedFormat.JSON => await json.Read(reader, (JsonTransferKey j) => (TransferKey)j),
            _ => throw new NotSupportedException($"Format {format} is not supported!")
        };
    }

    public async Task WriteTransfers(StreamWriter writer, IAsyncEnumerable<Transfer> transfers, SupportedFormat format, CancellationToken ct)
    {
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.Write<Transfer, TransferRow, TransferRowMap>(writer, transfers, t => (TransferRow)t, ct);
                return;

            case SupportedFormat.JSON:
                await json.Write(writer, transfers, ct);
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
                await csv.Write(writer, keys, t => (TransferKeyRow)t, ct);
                return;

            case SupportedFormat.JSON:
                await json.Write(writer, keys, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}