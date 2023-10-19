using JetBrains.Annotations;

namespace Flow.Plugins.Transactions.FlexibleTransactionsReader.Settings;

internal sealed class MappingRule
{
    public string? Field { get; [UsedImplicitly] set; }

    public int? ColumnId { get; [UsedImplicitly] set; }

    public string? FallbackValue { get; [UsedImplicitly] set; }
}
