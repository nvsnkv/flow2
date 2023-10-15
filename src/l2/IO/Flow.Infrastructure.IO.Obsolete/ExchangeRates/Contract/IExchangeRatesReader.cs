using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.ExchangeRates.Contract;

public interface IExchangeRatesReader
{
    Task<IEnumerable<ExchangeRate>> ReadExchangeRates(StreamReader reader, OldSupportedFormat format, CancellationToken ct);
}
