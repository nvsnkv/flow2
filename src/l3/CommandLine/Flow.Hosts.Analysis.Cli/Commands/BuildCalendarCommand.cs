﻿using Flow.Application.Analysis.Contract;
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
    private readonly IDimensionsParser parser;
    private readonly IAggregator aggregator;
    private readonly IRejectionsWriter rejectionsWriter;
    private readonly ICalendarWriter calendarWriter;

    public BuildCalendarCommand(IFlowConfiguration config, IDimensionsParser parser, IAggregator aggregator, IRejectionsWriter rejectionsWriter, ICalendarWriter calendarWriter) : base(config)
    {
        this.parser = parser;
        this.aggregator = aggregator;
        this.rejectionsWriter = rejectionsWriter;
        this.calendarWriter = calendarWriter;
    }

    public async Task<int> Execute(BuildCalendarArgs arg, CancellationToken ct)
    {
        var parsingResult = await parser.ParseFromStream(CreateReader(arg.DimensionsSetup), ct);
        if (!parsingResult.Successful)
        {
            foreach (var error in parsingResult.Errors)
            {
                await Console.Error.WriteLineAsync(error);
            }

            return 1;
        }

        var (calendar, rejections) = await aggregator.GetCalendar(arg.From, arg.Till, arg.Currency, parsingResult.Dimensions!, ct);

        var rejectionsWithCount = new EnumerableWithCount<RejectedTransaction>(rejections);

        await using (var rejWriter = CreateWriter(arg.RejectedPath))
        {
            await rejectionsWriter.WriteRejections(rejWriter, rejectionsWithCount, arg.Format, ct);
            if (rejectionsWithCount.Count > 0)
            {
                await TryStartEditor(arg.RejectedPath, arg.Format, false);
            }
        }

        await using var writer = CreateWriter(arg.OutputPath);
        await calendarWriter.WriteCalendar(writer, calendar, arg.Format, ct);

        await TryStartEditor(arg.OutputPath, arg.Format, false);
        return 0;
    }
}