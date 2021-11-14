using Flow.Application.ExchangeRates.Infrastructure;
using FLow.Domain.ExchangeRates;
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

    public async Task<IEnumerable<ExchangeRate>> Read(CancellationToken ct)
    {
        await using var context = contextFactory.CreateDbContext();
        return await context.ExchangeRates.ToListAsync(ct);
    }

    public async Task Create(ExchangeRate rate, CancellationToken ct)
    {
        await using var context = contextFactory.CreateDbContext();
        context.Add(rate);

        await context.SaveChangesAsync(ct);
    }
}