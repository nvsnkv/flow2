using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface IExchangeRatesReader
{
    Task<IEnumerable<ExchangeRate>> ReadExchangeRates(StreamReader reader, SupportedFormat format, CancellationToken ct);
}