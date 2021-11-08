using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions;

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
}