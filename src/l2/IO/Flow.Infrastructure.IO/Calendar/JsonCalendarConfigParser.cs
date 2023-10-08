using System.Linq.Expressions;
using Flow.Domain.Analysis;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Patterns;
using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Calendar.Contract;
using Flow.Infrastructure.IO.Generics;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Infrastructure.IO.Calendar;

internal class JsonCalendarConfigParser : ICalendarConfigParser
{
    private readonly JsonSerializer serializer;
    private readonly ITransactionCriteriaParser criteriaParser;
    
    public JsonCalendarConfigParser(JsonSerializer serializer, ITransactionCriteriaParser criteriaParser)
    {
        this.serializer = serializer;
        this.criteriaParser = criteriaParser;
    }

    public async Task<CalendarConfigParsingResult> ParseFromStream(StreamReader reader, CancellationToken ct)
    {
        JsonCalendarConfig? config;
        try
        {
            config = await serializer.Read<JsonCalendarConfig>(reader);
        }
        catch (Exception e)
        {
            return new CalendarConfigParsingResult("Failed to parse JSON!", e.ToString());
        }

        if (config == null)
        {
            return new CalendarConfigParsingResult("Failed to parse JSON! Serializer returned empty result.");
        }

        if (config.Dimensions == null)
        {
            return new CalendarConfigParsingResult("No dimensions provided for calendar config.");
        }

        var errors = new List<string>();
        var series = ParseSeries(config.Series, errors, string.Empty);
        if (series == null)
        {
            errors.Add("No series configs were parsed");
            return new CalendarConfigParsingResult(errors);
        }

        if (errors.Any())
        {
            return new CalendarConfigParsingResult(errors);
        }

        return new CalendarConfigParsingResult(new CalendarConfig(series, new Vector(config.Dimensions)));
        
    }

    private IReadOnlyList<SeriesConfig>? ParseSeries(List<JsonSeriesConfig>? series, List<string> errors, string position)
    {
        return series?.Select((c, i) => ParseSingleConfig(c, errors, i, position))
            .Where(s => s != null)
            .Cast<SeriesConfig>()
            .ToList()
            .AsReadOnly();
    }

    private SeriesConfig? ParseSingleConfig(JsonSeriesConfig config, List<string> errors, int order, string position)
    {
        if (config.Measurement is null)
        {
            errors.Add($"[Setup {position}:{order}] Measurement was not provided for series config.");
            return null;
        }

        if (config.Rules == null && config.Rule == null)
        {
            errors.Add($"[Setup {position}:{order}] No rules defined.");
            return null;
        }

        var rules = GetRules(config.Rules, config.Rule, errors, order, position)
            .Select(r => r.Compile())
            .ToList()
            .AsReadOnly();

        if (!rules.Any())
        {
            errors.Add($"[Setup {position}:{order}] No valid rules found.");
            return null;
        }

        return new SeriesConfig(new Vector(config.Measurement), rules, ParseSeries(config.SubSeries, errors, $"{position}:{order}"));
    }

    private IEnumerable<Expression<Func<RecordedTransaction, bool>>> GetRules(IEnumerable<string>? rules, string? rule, List<string> errors, int order, string position)
    {
        rules ??= Enumerable.Empty<string>();

        if (rule != null)
        {
            rules = rules.Prepend(rule);
        }

        var index = 0;
        foreach (var r in rules)
        {
            if (string.IsNullOrEmpty(r))
            {
                errors.Add($"[Setup {position}:{order}] Rule {index} - empty rule given!");
            }
            else if (r.Equals("ELSE"))
            {
                yield return Constants<RecordedTransaction>.Truth;
            }
            else
            {
                var result = criteriaParser.ParseRecordedTransactionCriteria(r);
                if (!result.Successful)
                {
                    errors.AddRange(result.Errors.Select(e => $"[Setup {position}:{order}] Rule {index} - {e}"));
                }
                else
                {
                    yield return result.Conditions!;
                }
            }

            index++;
        }


    }
}