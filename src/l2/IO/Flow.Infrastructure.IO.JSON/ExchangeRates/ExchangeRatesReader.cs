using Flow.Domain.ExchangeRates;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.JSON.ExchangeRates;

internal class ExchangeRatesReader : JsonReader<ExchangeRate, JsonExchangeRate>
{
    public ExchangeRatesReader(JsonSerializerSettings? settings) : base(settings, j => (ExchangeRate)j)
    {
    }
}
