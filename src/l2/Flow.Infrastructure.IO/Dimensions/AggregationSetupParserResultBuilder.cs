using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Flow.Domain.Analysis;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Patterns;
using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Dimensions;

internal class AggregationSetupParserResultBuilder
{
    private readonly Regex groupNamePattern = new("^group:\\s*(.*)$", RegexOptions.Compiled);
    private readonly Regex subGroupPattern = new("^subgroup:\\s*(.*)$", RegexOptions.Compiled);
    private readonly char separator;
    private readonly StreamReader reader;
    private readonly ITransactionCriteriaParser criteriaParser;
    private readonly CancellationToken ct;

    private int line;
    private int groupsFound;
    private string? text;

    public AggregationSetupParserResultBuilder(char separator, StreamReader reader, ITransactionCriteriaParser criteriaParser, CancellationToken ct)
    {
        this.separator = separator;
        this.reader = reader;
        this.ct = ct;
        this.criteriaParser = criteriaParser;
    }

    public async Task<AggregationSetupParsingResult> Build()
    {
        var header = await ParseHeader();
        
        if (ct.IsCancellationRequested)
        {
            return new AggregationSetupParsingResult("!: Task cancelled!");
        }

        if (header == null)
        {
            return new AggregationSetupParsingResult($"{line}: Unable to parse headers!");
        }

        var dimensionality = header.Length;
        var errors = new List<string>();

        var groups = await ParseGroups(dimensionality, e => errors.Add(e), ct).ToListAsync(ct);

        return errors.Count > 0
            ? new AggregationSetupParsingResult(errors)
            : new AggregationSetupParsingResult(new AggregationSetup(groups.Where(g => g != null).ToList().AsReadOnly()!, header));
    }

    private async IAsyncEnumerable<AggregationGroup?> ParseGroups(int dimensionality, Action<string> errorHandler, [EnumeratorCancellation] CancellationToken ct)
    {
        text = await ReadLine();
        while (!ct.IsCancellationRequested && !reader.EndOfStream)
        {
            var group = await ParseGroup(dimensionality, errorHandler);
            if (group != null)
            {
                yield return group;
            }
        }

        if (ct.IsCancellationRequested)
        {
            errorHandler("!: Task cancelled!");
        }

    }

    private async Task<AggregationGroup?> ParseGroup(int dimensionality, Action<string> errorHandler)
    {
        if (text == null)
        {
            errorHandler($"{line}: null value has been read!");
            return null;
        }

        var (name, lineProcessed) = GetGroupName(text);
        var rules = new List<AggregationRuleRow>();
        var subGroupDetected = false;

        while (!ct.IsCancellationRequested && !reader.EndOfStream)
        {
            if (lineProcessed)
            {
                text = await ReadLine();
            }

            lineProcessed = true;
            if (string.IsNullOrEmpty(text))
            {
                errorHandler($"{line}: Empty string detected!");
                continue;
            }

            subGroupDetected = subGroupPattern.IsMatch(text);
            if (groupNamePattern.IsMatch(text) || subGroupDetected)
            {
                break;
            }

            var split = text.Split(separator, StringSplitOptions.TrimEntries);
            if (split.Length != dimensionality + 1)
            {
                errorHandler($"{line}: Failed to parse aggregation rule! Rule must contain have same amount of dimensions as defined in header and must ends with transaction criteria!");
                continue;
            }

            var dimensions = split.Take(dimensionality);
            var ruleString = split.Last();

            var parseResult = criteriaParser.ParseRecordedTransactionCriteria(ruleString);
            if (!parseResult.Successful)
            {
                errorHandler($"{line}: Failed to parse criteria for [{string.Join(", ", dimensions)}]: {Environment.NewLine}  {string.Join(", ", parseResult.Errors)}");
            }
            else
            {
                rules.Add(new AggregationRuleRow(new Vector(dimensions), parseResult.Conditions!));
            }
        }

        var result = rules
            .GroupBy(r => r.Dimensions)
            .Select(g =>
                new AggregationRule(
                    g.Key,
                    g.Select(r => r.Rule)
                        .Aggregate((PatternBuilder<RecordedTransaction>)new OrPatternBuilder<RecordedTransaction>(),
                            (b, r) => b.With(r))
                        .Build()
                        .Compile()
                )
            );

        AggregationGroup? subgroup =  null;
        if (subGroupDetected)
        {
            subgroup = await ParseGroup(dimensionality, errorHandler);
        }

        groupsFound++;
        return new AggregationGroup(name, result.ToList().AsReadOnly(), subgroup);
    }

    private (string , bool) GetGroupName(string text)
    {
        var match = groupNamePattern.Match(text);
        if (!match.Success)
        {
            match = subGroupPattern.Match(text);
        }

        return (match.Success ? match.Groups[1].Value : $"Group {groupsFound}", match.Success);
    }

    private async Task<Vector?> ParseHeader()
    {
        text = await ReadLine();
        if (text == null) { return null; }

        var chunks = text.Split(separator, StringSplitOptions.TrimEntries);
        return chunks.Length == 0 ? null : new Vector(chunks);
    }

    private async Task<string?> ReadLine()
    {
        var result = await reader.ReadLineAsync();
        line++;
        return result?.Trim();
    }
}