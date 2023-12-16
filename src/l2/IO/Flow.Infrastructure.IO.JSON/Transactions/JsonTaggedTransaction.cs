using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.JSON.Transactions;

internal class JsonTaggedTransaction : JsonRecordedTransaction
{
    public string[]? Tags { get; set; }

    public static explicit operator JsonTaggedTransaction(TaggedTransaction t)
    {
        var result = new JsonTaggedTransaction
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

        result.Tags = t.Tags.Select(tg => tg.Name).ToArray();

        return result;
    }

    public static explicit operator TaggedTransaction(JsonTaggedTransaction js)
    {
        var transaction = (RecordedTransaction)(JsonRecordedTransaction)js;
        var tags = (js.Tags ?? Enumerable.Empty<string>()).Select(n => new Tag(n));
        var result = new TaggedTransaction(transaction, tags);

        return result;
    }
}
