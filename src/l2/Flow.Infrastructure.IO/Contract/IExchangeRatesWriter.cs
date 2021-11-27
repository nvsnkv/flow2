using Flow.Domain.ExchangeRates;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface IExchangeRatesWriter
{
    Task WriteRates(StreamWriter writer, IAsyncEnumerable<ExchangeRate> rates, SupportedFormat format, CancellationToken ct);
}