using System.Text.Json.Nodes;
using Flow.Domain.Transactions;
using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.Json;

internal class JsonRecordedTransaction : JsonTransaction
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
        var result = new RecordedTransaction(js.Key ?? default, transaction);

        if (!string.IsNullOrEmpty(js.Overrides?.Comment) ||
            !string.IsNullOrEmpty(js.Overrides?.Category) ||
            !string.IsNullOrEmpty(js.Overrides?.Title))
        {
            result.Overrides = new Overrides(js.Overrides?.Category, js.Overrides?.Title, js.Overrides?.Comment);
        }

        return result;
    }

    public static explicit operator JsonRecordedTransaction(RecordedTransaction t)
    {
        var result = new JsonRecordedTransaction
        {
            Account = new JsonAccountInfo
            {
                Bank = t.Account.Bank,
                Name = t.Account.Name
            },

            Amount = t.Amount,
            Category = t.Category,
            Currency = t.Currency,
            Key = t.Key,
            Timestamp = t.Timestamp,
            Title = t.Title
        };

        if (t.Overrides != null)
        {
            result.Overrides = new JsonOverride
            {
                Category = t.Overrides.Category,
                Comment = t.Overrides.Comment,
                Title = t.Overrides.Title
            };
        }

        return result;
    }
}