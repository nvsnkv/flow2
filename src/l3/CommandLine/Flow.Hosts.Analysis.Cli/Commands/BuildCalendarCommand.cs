using Flow.Application.Analysis.Contract;
using Flow.Domain.Common.Collections;
using Flow.Domain.Transactions;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Analysis.Cli.Commands;

[UsedImplicitly]
internal class BuildCalendarCommand :CommandBase
{
    private readonly IAggregationSetupParser parser;
    private readonly IAggregator aggregator;
    private readonly IRejectionsWriter rejectionsWriter;
    private readonly ICalendarWriter calendarWriter;

    public BuildCalendarCommand(IFlowConfiguration config, IAggregationSetupParser parser, IAggregator aggregator, IRejectionsWriter rejectionsWriter, ICalendarWriter calendarWriter) : base(config)
    {
        this.parser = parser;
        this.aggregator = aggregator;
        this.rejectionsWriter = rejectionsWriter;
        this.calendarWriter = calendarWriter;
    }

    public async Task<int> Execute(BuildCalendarArgs arg, CancellationToken ct)
    {
        AggregationSetupParsingResult parsingResult;
        using (var streamReader = CreateReader(arg.DimensionsSetup))
        {
            parsingResult = await parser.ParseFromStream(streamReader, ct);
            if (!parsingResult.Successful)
            {
                foreach (var error in parsingResult.Errors)
                {
                    await Console.Error.WriteLineAsync(error);
                }

                return 1;
            }
        }

        var (calendar, rejections) = await aggregator.GetCalendar(arg.From.ToUniversalTime(), arg.Till.ToUniversalTime(), arg.Currency, parsingResult.Setup!, ct);

        var rejectionsWithCount = new EnumerableWithCount<RejectedTransaction>(rejections);
        
        var rejectedPath = arg.RejectedPath ?? GetFallbackOutputPath(arg.Format, "flow", "rejected");
        await using (var rejWriter = CreateWriter(rejectedPath))
        {
            await rejectionsWriter.WriteRejections(rejWriter, rejectionsWithCount, arg.Format, ct);
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