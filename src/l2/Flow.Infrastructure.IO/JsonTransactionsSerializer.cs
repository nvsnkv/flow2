using System.Diagnostics.CodeAnalysis;
using Flow.Domain.Transactions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO;

internal class JsonTransactionsSerializer
{
    private readonly JsonSerializer serializer;

    public JsonTransactionsSerializer(JsonSerializerSettings? settings)
    {
        this.serializer = settings is null 
            ? JsonSerializer.CreateDefault() 
            : JsonSerializer.Create(settings);
    }

    public Task<IEnumerable<Transaction>> ReadTransactions(StreamReader reader)
    {
        var result = Read<JsonTransaction>(reader).Select(j => (Transaction)j);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader)
    {
        var result = Read<JsonRecordedTransaction>(reader).Select(j => (RecordedTransaction)j);
        return Task.FromResult(result);
    }

    private IEnumerable<T> Read<T>(StreamReader reader) where T :JsonTransaction
    {
        using var jsonReader = new JsonTextReader(reader) { CloseInput = false };
        var result = serializer.Deserialize<List<T>>(jsonReader) ?? Enumerable.Empty<T>();

        return result;
    }

    public async Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }

    public async Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        await Write(writer, transactions, ct);
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejected, CancellationToken ct)
    {
        await Write(writer, rejected, ct);
    }

    private async Task Write<T>(StreamWriter writer, IEnumerable<T> transactions, CancellationToken ct)
    {
        using var jsonWriter = new JsonTextWriter(writer) { CloseOutput = false };
        serializer.Serialize(jsonWriter, transactions);
        await jsonWriter.FlushAsync(ct);
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class JsonTransaction
    {
        public DateTime? Timestamp { get; set; }

        public decimal? Amount { get; set; }

        public string? Currency { get; set; }

        public string? Category { get; set; }

        public string? Title { get; set; }

        public JsonAccountInfo? Account { get; set; }

        [UsedImplicitly]
        public class JsonAccountInfo
        {
            public string? Name { get; set; }

            public string? Bank { get; set; }
        }

        public static explicit operator Transaction(JsonTransaction js)
        {
            var acc = new AccountInfo(js.Account?.Name ?? string.Empty, js.Account?.Bank ?? string.Empty);
            return new Transaction(js.Timestamp ?? default(DateTime), js.Amount ?? default(decimal), js.Currency ?? string.Empty, js.Category, js.Title ?? string.Empty, acc);
        }
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [UsedImplicitly]
    private class JsonRecordedTransaction: JsonTransaction
    {
        public long? Key { get; set; }

        public JsonOverride? Overrides { get; set; }

        [UsedImplicitly]
        public class JsonOverride
        {
            public string? Comment { get; set; }

            public string? Category { get; set; }

            public string? Title { get; set; }
        }

        public static explicit operator RecordedTransaction(JsonRecordedTransaction js)
        {
            var transaction = (Transaction)(JsonTransaction)js;
            var result = new RecordedTransaction(js.Key ?? default(long), transaction);

            if (!string.IsNullOrEmpty(js.Overrides?.Comment) || 
                !string.IsNullOrEmpty(js.Overrides?.Category) ||
                !string.IsNullOrEmpty(js.Overrides?.Title))
            {
                result.Overrides = new Overrides(js.Overrides?.Category, js.Overrides?.Title, js.Overrides?.Comment);
            }

            return result;
        }
    }
}