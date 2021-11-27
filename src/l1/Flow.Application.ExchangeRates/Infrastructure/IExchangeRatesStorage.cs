using Flow.Domain.ExchangeRates;

namespace Flow.Application.ExchangeRates.Infrastructure;

public interface IExchangeRatesStorage
{
    Task Create(ExchangeRate rate, CancellationToken ct);

    IAsyncEnumerable<ExchangeRate> Read(CancellationToken ct);

    Task Update(IEnumerable<ExchangeRate> rates, CancellationToken ct);

    Task Delete(IEnumerable<ExchangeRate> rates, CancellationToken ct);
}