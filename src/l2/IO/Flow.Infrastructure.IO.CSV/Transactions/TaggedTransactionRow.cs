using System.Diagnostics.CodeAnalysis;
using System.Text;
using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.CSV.Transactions;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class TaggedTransactionRow : RecordedTransactionRow
{
    public string? TAGS { get; set; }

    public static explicit operator TaggedTransaction(TaggedTransactionRow row)
    {
        var recorded = (RecordedTransaction)(RecordedTransactionRow)row;
        var tags = row.TAGS?.Split(',').Select(t => new Tag(t.Trim())) ?? Enumerable.Empty<Tag>();

        return new TaggedTransaction(recorded, tags);
    }

    public static explicit operator TaggedTransactionRow(TaggedTransaction t)
    {
        return new TaggedTransactionRow()
        {
            TIMESTAMP = t.Timestamp.ToLocalTime(),
            AMOUNT = t.Amount,
            CURRENCY = t.Currency,
            CATEGORY = t.Category,
            TITLE = t.Title,
            ACCOUNT = t.Account.Name,
            BANK = t.Account.Bank,
            KEY = t.Key,
            CATEGORY_OVERRIDE = t.Overrides?.Category,
            TITLE_OVERRIDE = t.Overrides?.Title,
            COMMENT = t.Overrides?.Comment,
            REVISION = t.Revision,
            TAGS = t.Tags.Aggregate(new StringBuilder(), (b, t)=> b.Append(t.Name).Append(", ")).ToString()
        };
    }
}
