using Flow.Domain.Analysis;

namespace Flow.Application.Analysis.Contract.Setup;

public record CalendarConfig(
    IReadOnlyList<SeriesConfig> Series,
    Vector Dimensions,
    int? Depth = null);
