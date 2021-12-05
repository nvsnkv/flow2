namespace Flow.Infrastructure.IO.Contract;

public interface IAggregationSetupParser
{
    Task<AggregationSetupParsingResult> ParseFromStream(StreamReader reader, CancellationToken ct);
}