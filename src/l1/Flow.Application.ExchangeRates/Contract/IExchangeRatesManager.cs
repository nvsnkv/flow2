using Flow.Domain.ExchangeRates;

namespace Flow.Application.ExchangeRates.Contract;

public interface IExchangeRatesManager
{
    Task<ExchangeRate?> Request(ExchangeRateRequest request, CancellationToken ct);

    IAsyncEnumerable<ExchangeRate> List(CancellationToken ct);

    Task<IEnumerable<RejectedRate>> Update(IEnumerable<ExchangeRate> rates, CancellationToken ct);

    Task<IEnumerable<RejectedRate>> Delete(IEnumerable<ExchangeRate> rates, CancellationToken ct);
}