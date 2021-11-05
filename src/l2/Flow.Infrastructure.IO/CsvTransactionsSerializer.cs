using System.Diagnostics.CodeAnalysis;
using CsvHelper;
using CsvHelper.Configuration;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO;

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
        await csvWriter.WriteRecordsAsync(transactions.Select(t => (TransactionRow)t), ct);
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions,  CancellationToken ct)
    {
        await using var csvWriter = new CsvWriter(writer, config);
        await csvWriter.WriteRecordsAsync(transactions.Select(t => (RecordedTransactionRow)t), ct);
    }

    public async Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, config);
        var result = new List<Transaction>();

        await foreach (var row in csvReader.GetRecordsAsync(typeof(TransactionRow), ct))
        {
            result.Add((Transaction)row);
        }

        return result;
    }

    public async Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, CancellationToken ct)
    {
        using var csvReader = new CsvReader(reader, config);
        var result = new List<RecordedTransaction>();

        await foreach (var row in csvReader.GetRecordsAsync(typeof(RecordedTransactionRow), ct))
        {
            result.Add((RecordedTransaction)row);
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

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private class TransactionRow
    {
        public DateTime? TIMESTAMP { get; set; }

        public decimal? AMOUNT { get; set; }

        public string? CURRENCY { get; set; }

        public string? CATEGORY { get; set; }

        public string? TITLE { get; set; }

        public string? ACCOUNT { get; set; }

        public string? BANK { get; set; }

        public static explicit operator Transaction(TransactionRow row)
        {
            return new Transaction(row.TIMESTAMP ?? default, row.AMOUNT ?? default, row.CURRENCY ?? string.Empty, row.CATEGORY, row.TITLE ?? string.Empty, new AccountInfo(row.ACCOUNT ?? string.Empty, row.BANK ?? string.Empty));
        }

        public static explicit operator TransactionRow(Transaction t)
        {
            return new TransactionRow()
            {
                TIMESTAMP = t.Timestamp,
                AMOUNT = t.Amount,
                CURRENCY = t.Currency,
                CATEGORY = t.Category,
                TITLE = t.Title,
                ACCOUNT = t.Account.Name,
                BANK = t.Account.Bank
            };
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private class RecordedTransactionRow : TransactionRow
    {   
        public long? KEY { get; set; }

        public string? COMMENT { get; set; }

        public string? CATEGORY_OVERRIDE { get; set; }

        public string? TITLE_OVERRIDE { get; set; }

        public static explicit operator RecordedTransaction(RecordedTransactionRow row)
        {
            var transaction = (Transaction)(TransactionRow)row;
            var result = new RecordedTransaction(row.KEY ?? default, transaction);

            if (!string.IsNullOrEmpty(row.COMMENT) || !string.IsNullOrEmpty(row.CATEGORY_OVERRIDE) || !string.IsNullOrEmpty(row.TITLE_OVERRIDE))
            {
                result.Overrides = new Overrides(row.CATEGORY_OVERRIDE, row.TITLE_OVERRIDE, row.COMMENT);
            }

            return result;
        }

        public static explicit operator RecordedTransactionRow(RecordedTransaction t)
        {
            return new RecordedTransactionRow()
            {
                TIMESTAMP = t.Timestamp,
                AMOUNT = t.Amount,
                CURRENCY = t.Currency,
                CATEGORY = t.Category,
                TITLE = t.Title,
                ACCOUNT = t.Account.Name,
                BANK = t.Account.Bank,
                KEY = t.Key,
                CATEGORY_OVERRIDE = t.Overrides?.Category,
                TITLE_OVERRIDE = t.Overrides?.Title
            };
        }
    }
}