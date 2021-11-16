using Flow.Domain.Transactions.Transfers;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class TransfersIOFacade : ITransferKeysReader, ITransfersWriter
{
    private readonly CsvTransfersSerializer csv;
    private readonly JsonSerializer json;

    public TransfersIOFacade(CsvTransfersSerializer csv, JsonSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task<IEnumerable<TransferKey>> ReadTransferKeys(StreamReader reader, SupportedFormat format, CancellationToken ct)
    {
        return format switch
        {
            SupportedFormat.CSV => await csv.ReadTransferKeys(reader, ct),
            SupportedFormat.JSON => await json.Read(reader, (JsonTransferKey j) => (TransferKey)j),
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
                await csv.WriteTransferKeys(writer, keys, ct);
                return;

            case SupportedFormat.JSON:
                await json.Write(writer, keys, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}