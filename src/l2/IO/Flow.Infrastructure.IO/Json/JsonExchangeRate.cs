using Flow.Domain.ExchangeRates;

namespace Flow.Infrastructure.IO.Json;

internal class JsonExchangeRate
{
    public string? From { get; set; }

    public string? To { get; set; }

    public DateTime? Date { get; set; }

    public decimal? Rate { get; set; }

    public static explicit operator ExchangeRate(JsonExchangeRate j)
    {
        return new ExchangeRate(j.From ?? string.Empty, j.To ?? string.Empty, j.Date ?? default, j.Rate ?? 0);
    }
}