using Flow.Domain.Analysis;

namespace Flow.Infrastructure.IO.Contract;

public class AggregationSetupParsingResult
{
    public AggregationSetupParsingResult(AggregationSetup? setup)
    {
        Setup = setup;
        Errors = Enumerable.Empty<string>();
    }

    public AggregationSetupParsingResult(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    public AggregationSetupParsingResult(params string[] errors)
    {
        Errors = errors;
    }

    public AggregationSetup? Setup { get; }

    public bool Successful => Setup != null;

    public IEnumerable<string> Errors { get; }
}