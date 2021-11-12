using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Csv;

internal class CsvTransactionsSerializer
{
    private readonly CsvConfiguration config;

    public CsvTransactionsSerializer(CsvConfiguration config)
    {
        this.config = config;
    }

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        csvWriter.Context.RegisterClassMap<TransactionRowMap>();

        await csvWriter.WriteRecordsAsync(transactions.Select(t => (TransactionRow)t), ct);
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions,  CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        csvWriter.Context.RegisterClassMap<RecordedTransactionRowMap>();

        await csvWriter.WriteRecordsAsync(transactions.Select(t => (RecordedTransactionRow)t), ct);
    }

    public async Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, config);
        var result = new List<Transaction>();

        await foreach (var row in csvReader.GetRecordsAsync(typeof(TransactionRow), ct))
        {
            result.Add((Transaction)(TransactionRow)row);
        }

        return result;
    }

    public async Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, config);
        var result = new List<RecordedTransaction>();

        await foreach (var row in csvReader.GetRecordsAsync(typeof(RecordedTransactionRow), ct))
        {
            result.Add((RecordedTransaction)(RecordedTransactionRow)row);
        }

        return result;
    }

    public async Task WriterTransfers(StreamWriter writer, IEnumerable<Transfer> transfers, CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        csvWriter.Context.RegisterClassMap<TransferRowMap>();

        await csvWriter.WriteRecordsAsync(transfers.Select(k => (TransferRow)k), ct);
    }

    public async Task WriterTransferKeys(StreamWriter writer, IEnumerable<TransferKey> keys, CancellationToken ct)
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

            csvWriter.WriteRecord((TransferKeyRow)rejected.Entity);

            foreach (var reason in rejected.Reasons)
            {
                await writer.WriteLineAsync(reason.ToCharArray(), ct);
            }
        }
    }
}