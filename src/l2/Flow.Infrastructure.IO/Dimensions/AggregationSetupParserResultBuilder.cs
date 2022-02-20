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
    private readonly Regex sectionNamePattern = new("^section:\\s*(.*)$", RegexOptions.Compiled);
    private readonly Regex altSectionPattern = new("^alt section:\\s*(.*)$", RegexOptions.Compiled);
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

        var sections = await ParseSections(dimensionality, e => errors.Add(e), ct).ToListAsync(ct);

        return errors.Count > 0
            ? new AggregationSetupParsingResult(errors)
            : new AggregationSetupParsingResult(new AggregationSetup(sections.Where(g => g != null).ToList().AsReadOnly()!, header));
    }

    private async IAsyncEnumerable<SectionSetup?> ParseSections(int dimensionality, Action<string> errorHandler, [EnumeratorCancellation] CancellationToken ct)
    {
        text = await ReadLine();
        while (!ct.IsCancellationRequested && !reader.EndOfStream)
        {
            var section = await ParseSection(dimensionality, errorHandler);
            if (section != null)
            {
                yield return section;
            }
        }

        if (ct.IsCancellationRequested)
        {
            errorHandler("!: Task cancelled!");
        }
    }

    private async Task<SectionSetup?> ParseSection(int dimensionality, Action<string> errorHandler)
    {
        if (text == null)
        {
            errorHandler($"{line}: null value has been read!");
            return null;
        }

        var (name, lineProcessed) = GetSectionName(text);
        var rules = new List<AggregationRuleRow>();
        var altSectionDetected = false;

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

            altSectionDetected = altSectionPattern.IsMatch(text);
            if (sectionNamePattern.IsMatch(text) || altSectionDetected)
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
                new SectionRule(
                    g.Key,
                    g.Select(r => r.Rule)
                        .Aggregate((PatternBuilder<RecordedTransaction>)new OrPatternBuilder<RecordedTransaction>(),
                            (b, r) => b.With(r))
                        .Build()
                        .Compile()
                )
            );

        SectionSetup? alternative =  null;
        if (altSectionDetected)
        {
            alternative = await ParseSection(dimensionality, errorHandler);
        }

        groupsFound++;
        return new SectionSetup(name, result.ToList().AsReadOnly(), alternative);
    }

    private (string , bool) GetSectionName(string text)
    {
        var match = sectionNamePattern.Match(text);
        if (!match.Success)
        {
            match = altSectionPattern.Match(text);
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