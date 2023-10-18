namespace Flow.Plugins.Transactions.FlexibleTransactionsReader.Settings;

internal sealed class MappingRule
{
    public string Field { get; set; }

    public int? ColumnId { get; set; }

    public string? FallbackValue { get; set; }
}
