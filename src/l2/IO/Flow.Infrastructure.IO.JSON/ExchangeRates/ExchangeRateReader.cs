using Flow.Domain.ExchangeRates;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON.ExchangeRates;

internal class ExchangeRateReader : JsonReader<ExchangeRate, JsonExchangeRate>
{
    public ExchangeRateReader(JsonSerializerSettings? settings) : base(settings, j => (ExchangeRate)j)
    {
    }
}
