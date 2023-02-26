namespace Flow.Domain.Analysis.Setup;

public record CalendarConfig(
    IReadOnlyList<SeriesConfig> Series,
    Vector Dimensions,
    int? Depth = null);