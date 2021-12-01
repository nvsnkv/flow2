namespace Flow.Infrastructure.IO.Contract;

public interface IDimensionsParser
{
    Task<DimensionsParsingResult> ParseFromStream(StreamReader reader, CancellationToken ct);
}