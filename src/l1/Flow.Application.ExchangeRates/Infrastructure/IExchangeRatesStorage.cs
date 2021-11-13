using FLow.Domain.ExchangeRates;

namespace Flow.Application.ExchangeRates.Infrastructure;

public interface IExchangeRatesStorage
{
    Task<IEnumerable<ExchangeRate>> Read(CancellationToken ct);

    Task Create(ExchangeRate rate, CancellationToken ct);
}