using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Dimensions;

internal class AggregationSetupParser
{
    private readonly ITransactionCriteriaParser criteriaParser;
    private readonly char separator;

    public AggregationSetupParser(char separator, ITransactionCriteriaParser criteriaParser)
    {
        this.separator = separator;
        this.criteriaParser = criteriaParser;
    }

    public async Task<AggregationSetupParsingResult> ParseFromStream(StreamReader reader, CancellationToken ct)
    {
        return await new AggregationSetupParserResultBuilder(separator, reader, criteriaParser, ct).Build();

    }
}