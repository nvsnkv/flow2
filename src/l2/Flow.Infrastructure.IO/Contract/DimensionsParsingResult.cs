using Flow.Domain.Analysis;

namespace Flow.Infrastructure.IO.Contract;

public class DimensionsParsingResult
{
    public DimensionsParsingResult(IEnumerable<Dimension>? dimensions, IEnumerable<string>? errors)
    {
        Dimensions = dimensions;
        Errors = errors ?? Enumerable.Empty<string>();
    }

    public IEnumerable<Dimension>? Dimensions { get; }

    public bool Successful => Dimensions != null;

    public IEnumerable<string> Errors { get; }
}