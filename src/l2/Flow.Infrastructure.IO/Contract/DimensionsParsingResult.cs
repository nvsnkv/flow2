using Flow.Domain.Analysis;

namespace Flow.Infrastructure.IO.Contract;

[Obsolete("Use AggregationSetupParsingResult instead")]
public class DimensionsParsingResult
{
    public DimensionsParsingResult(Vector? header, IEnumerable<AggregationRule>? dimensions, IEnumerable<string>? errors)
    {
        Header = header ?? Vector.Empty;
        Dimensions = dimensions;
        Errors = errors ?? Enumerable.Empty<string>();
    }

    public Vector Header { get; }

    public IEnumerable<AggregationRule>? Dimensions { get; }

    public bool Successful => Dimensions != null;

    public IEnumerable<string> Errors { get; }
}