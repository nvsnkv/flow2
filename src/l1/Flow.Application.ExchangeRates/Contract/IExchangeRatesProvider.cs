using Flow.Domain.ExchangeRates;

namespace Flow.Application.ExchangeRates.Contract;
public interface IExchangeRatesProvider
{
    Task<ExchangeRate?> GetRate(ExchangeRateRequest request, CancellationToken ct);
}