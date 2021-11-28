using Flow.Domain.Analysis;

namespace Flow.Infrastructure.IO.Csv;

internal class FlowRow
{
    public string? TYPE { get; init; }

    public long KEY { get; init; }

    public DateTime TIMESTAMP { get; init; }

    public decimal AMOUNT { get; init; }

    public string? CURRENCY { get; init; }

    public string? CATEGORY { get; init; }

    public string? TITLE { get; init; }

    public static explicit operator FlowRow(FlowItem item) 
    {
        return new FlowRow 
        {
            TYPE = item is Income ? "+" : "-",
            KEY = item.Key,
            TIMESTAMP = item.Timestamp,
            AMOUNT = item.Amount,
            CURRENCY = item.Currency,
            CATEGORY = item.Category,
            TITLE = item.Title
        };
    }
}
