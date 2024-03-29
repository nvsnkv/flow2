﻿using Flow.Application.ExchangeRates.Contract;
using Flow.Domain.ExchangeRates;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.ExchangeRates.Cli.Commands;

[UsedImplicitly]
internal class ListCommand : CommandBase
{
    private readonly IExchangeRatesManager manager;
    private readonly IWriters<ExchangeRate> writers;

    public ListCommand(IFlowConfiguration config, IExchangeRatesManager manager, IWriters<ExchangeRate> writers) : base(config)
    {
        this.manager = manager;
        this.writers = writers;
    }

    public async Task<int> Execute(RequestArgs args, CancellationToken ct)
    { 
        var request = (args.From ?? string.Empty, args.To ?? string.Empty, args.Date);
        var rate = await manager.Request(request, ct);

        if (rate != null)
        {
            var rates = Enumerable.Repeat(rate, 1);
            return await WriteRates(rates, args.Format, null, ct);
        }

        return -1;
    }

    public async Task<int> Execute(ListArgs args, CancellationToken ct)
    {
        var rates = await manager.List(ct);

        return await WriteRates(rates, args.Format, args.Output, ct);
    }

    private async Task<int> WriteRates(IEnumerable<ExchangeRate> rates, SupportedFormat format, string? output, CancellationToken ct)
    {
        await using var streamWriter = CreateWriter(output);
        await writers.GetFor(format).Write(streamWriter, rates, ct);

        return await TryStartEditor(output, format, false);
    }
}
