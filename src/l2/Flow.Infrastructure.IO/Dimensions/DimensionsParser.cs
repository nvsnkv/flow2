using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using Flow.Domain.Analysis;
using Flow.Domain.Patterns;
using Flow.Domain.Patterns.Logical;
using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Dimensions;

/// <summary>
/// Parses dimensions setup from following lines
/// {dimension};{value};{criteria}
/// where {dimension} defines dimension name, {value} - value within dimension and {criteria} is transaction selection criteria that can be parsed by <see cref="ITransactionCriteriaParser"/>
/// </summary>
internal class DimensionsParser : IDimensionsParser
{
    private readonly ITransactionCriteriaParser criteriaParser;

    public DimensionsParser(ITransactionCriteriaParser criteriaParser)
    {
        this.criteriaParser = criteriaParser;
    }

    public async Task<DimensionsParsingResult> ParseFromStream(StreamReader reader, CancellationToken ct)
    {
        var errors = new List<string>();
        var rules = new List<AggregationRuleRow>();
       

        var index = 0;

        if (reader.EndOfStream)
        {
            errors.Add("!: Empty input!");
            return new DimensionsParsingResult(null, null, errors);
        }

        var headerLine = await reader.ReadLineAsync();
        index++;

        var header = new Vector(headerLine?.Split(';'));
        if (header.Length < 1)
        {
            errors.Add($"{index}: No header defined!");
            return new DimensionsParsingResult(null, null, errors);
        }

        var dimensionality = header.Length;

        while (!reader.EndOfStream || ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            index++;
            if (string.IsNullOrEmpty(line))
            {
                errors.Add($"{index}: Empty line detected");
            }
            else
            {
                var split = line.Split(";");

                if (split.Length != dimensionality + 1)
                {
                    errors.Add($"{index}: Failed to parse aggregation rule! Rule must contain have same amount of dimensions as defined in header and must ends with transaction criteria!");
                }

                var dimensions = new Vector(split.Take(split.Length - 1).ToList());
                var value = split.Last();

                var criteriaResult = criteriaParser.ParseRecordedTransactionCriteria(value.Split(' '));
                if (!criteriaResult.Successful)
                {
                    errors.Add($"{index}: Failed to parse criteria for [{string.Join(", ", dimensions)}]: {Environment.NewLine}  {string.Join(", ", criteriaResult.Errors)}");
                }
                else
                {
                    if (dimensions.Length > dimensionality) { dimensionality = dimensions.Length; }
                    rules.Add(new AggregationRuleRow(dimensions, criteriaResult.Conditions!));
                }
            }
        }

        if (ct.IsCancellationRequested)
        {
            errors.Add("!: Cancellation requested!");
        }

        if (errors.Any())
        {
            return new DimensionsParsingResult(null, null, errors);
        }

        var result = rules
            .GroupBy(r => r.Dimensions)
            .Select(g => 
                new AggregationRule(
                    g.Key,
                    g.Select(r => r.Rule)
                          .Aggregate(Constants<RecordedTransaction>.Falsity, (s, r) => s.Or(r))
                          .Compile()
                    )
                )
            .Append(new AggregationRule(new Vector(Enumerable.Repeat(string.Empty, dimensionality)), Constants<RecordedTransaction>.Truth.Compile()));

        return new DimensionsParsingResult(header, result, null);
    }
}