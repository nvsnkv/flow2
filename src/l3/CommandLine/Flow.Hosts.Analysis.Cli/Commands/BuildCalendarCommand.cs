using Flow.Application.Analysis.Contract;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Calendar.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Criteria.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Analysis.Cli.Commands;

[UsedImplicitly]
internal class BuildCalendarCommand :CommandBase
{
    private readonly ICalendarConfigParser calendarConfigParser;
    private readonly ITransactionCriteriaParser criteriaParser;
    private readonly IAggregator aggregator;
    private readonly IWriters<RejectedTransaction> writers;
    private readonly ICalendarWriter calendarWriter;


    public BuildCalendarCommand(IFlowConfiguration config, ICalendarConfigParser calendarConfigParser, IAggregator aggregator, IWriters<RejectedTransaction> writers, ICalendarWriter calendarWriter, ITransactionCriteriaParser criteriaParser) : base(config)
    {
        this.calendarConfigParser = calendarConfigParser;
        this.aggregator = aggregator;
        this.writers = writers;
        this.calendarWriter = calendarWriter;
        this.criteriaParser = criteriaParser;
    }

    public async Task<int> Execute(BuildCalendarArgs arg, CancellationToken ct)
    {
        CalendarConfigParsingResult parsingResult;
        using (var streamReader = CreateReader(arg.SeriesSetup))
        {
            parsingResult = await calendarConfigParser.ParseFromStream(streamReader, ct);
            if (!parsingResult.Successful)
            {
                foreach (var error in parsingResult.Errors)
                {
                    await Console.Error.WriteLineAsync(error);
                }

                return 1;
            }
        }

        var criteria = criteriaParser.ParseRecordedTransactionCriteria(arg.Criteria ?? Enumerable.Empty<string>());
        if (!criteria.Successful)
        {
            foreach (var error in criteria.Errors)
            {
                await Console.Error.WriteLineAsync(error);
                return 1;
            }
        }

        var flowConfig = new FlowConfig(arg.From.ToUniversalTime(), arg.Till.ToUniversalTime(), arg.Currency, criteria.Conditions);
        var calendarConfig = parsingResult.Config! with { Depth = arg.Depth };

        var (calendar, rejections) = await aggregator.GetCalendar(flowConfig, calendarConfig, ct);

        var rejectionsWithCount = new EnumerableWithCount<RejectedTransaction>(rejections);
        
        var rejectedPath = arg.RejectedPath ?? GetFallbackOutputPath(arg.Format, "flow", "rejected");
        await using (var rejWriter = CreateWriter(rejectedPath))
        {
            await writers.GetFor(arg.Format).Write(rejWriter, rejectionsWithCount, ct);
            if (rejectionsWithCount.Count > 0)
            {
                await TryStartEditor(rejectedPath, arg.Format, false);
            }
        }

        var outputPath = arg.OutputPath ?? GetFallbackOutputPath(arg.Format, "flow", "calendar");
        await using var writer = CreateWriter(outputPath);
        await calendarWriter.WriteCalendar(writer, calendar, arg.Format, ct);

        await TryStartEditor(outputPath, arg.Format, false);
        return 0;
    }
}
