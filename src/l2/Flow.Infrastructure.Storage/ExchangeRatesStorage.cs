using System.Runtime.CompilerServices;
using Flow.Application.ExchangeRates.Infrastructure;
using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Storage.Model;
using Microsoft.EntityFrameworkCore;

namespace Flow.Infrastructure.Storage;

internal class ExchangeRatesStorage : IExchangeRatesStorage
{
    private readonly IDbContextFactory<FlowDbContext> contextFactory;

    public ExchangeRatesStorage(IDbContextFactory<FlowDbContext> contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async Task Create(ExchangeRate rate, CancellationToken ct)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Add(rate);

        await context.SaveChangesAsync(ct);
    }

    public async IAsyncEnumerable<ExchangeRate> Read([EnumeratorCancellation] CancellationToken ct)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        await foreach (var rate in context.ExchangeRates.AsAsyncEnumerable().WithCancellation(ct))
        {
            yield return rate;
        }
    }

    public async Task Update(IEnumerable<ExchangeRate> rates, CancellationToken ct)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        foreach (var rate in rates)
        {
            var target = await context.ExchangeRates.FirstOrDefaultAsync(r => r.From == rate.From && r.To == rate.To && r.Date == rate.Date, ct);
            if (target != null)
            {
                context.ExchangeRates.Remove(target);
            }

            await context.ExchangeRates.AddAsync(rate, ct);
        }

        await context.SaveChangesAsync(ct);
    }

    public async Task Delete(IEnumerable<ExchangeRate> rates, CancellationToken ct)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        foreach (var rate in rates)
        {
            var target = await context.ExchangeRates.FirstOrDefaultAsync(r => r.From == rate.From && r.To == rate.To && r.Date == rate.Date, ct);
            if (target != null)
            {
                context.ExchangeRates.Remove(target);
            }
        }

        await context.SaveChangesAsync(ct);
    }
}