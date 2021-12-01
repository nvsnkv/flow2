using System.Collections.ObjectModel;
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
        var rules = new List<DimensionRule>();

        var index = 0;
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
                if (split.Length != 3)
                {
                    errors.Add($"{index}: Failed to identify dimension name, value and rule. Please check if they properly separated by ;");
                }

                var dimension = split[0];
                var value = split[1];

                var criteriaResult = criteriaParser.ParseRecordedTransactionCriteria(split[2].Split(' '));
                if (!criteriaResult.Successful)
                {
                    errors.Add($"{index}: Failed to parse criteria for {dimension}={value}: {Environment.NewLine}  {string.Join(", ", criteriaResult.Errors)}");
                }
                else
                {
                    rules.Add(new DimensionRule(dimension, value, criteriaResult.Conditions!));
                }
            }
        }

        if (ct.IsCancellationRequested)
        {
            errors.Add("!: Cancellation requested!");
        }

        if (errors.Any())
        {
            return new DimensionsParsingResult(null, errors);
        }

        var result = rules
            .GroupBy(r => r.Name)
            .Select(g => new Dimension(
                g.Key, new ReadOnlyDictionary<string, Func<RecordedTransaction, bool>>(
                    g.GroupBy(r => r.Value)
                        .ToDictionary(
                            gv => gv.Key,
                            gv => gv.Select(r => r.Rule)
                                .Aggregate(Constants<RecordedTransaction>.Falsity, (s, r) => s.Or(r))
                                .Compile()
                        )
                    )
                )
            );

        return new DimensionsParsingResult(result, null);
    }
}