using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.ExchangeRates.Contract;

public interface IExchangeRatesWriter
{
    Task WriteRates(StreamWriter writer, IEnumerable<ExchangeRate> rates, OldSupportedFormat format, CancellationToken ct);
}
