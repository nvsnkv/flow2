using System.Diagnostics.CodeAnalysis;
using Flow.Domain.ExchangeRates;

namespace Flow.Infrastructure.IO.Csv;

[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class ExchangeRateRow
{
    public string? FROM { get; init; }

    public string? TO { get; init; }

    public DateTime? DATE { get; init; }

    public decimal? RATE { get; init; }

    public static explicit operator ExchangeRateRow(ExchangeRate rate)
    {
        return new ExchangeRateRow
        {
            FROM = rate.From,
            TO = rate.To,
            DATE = rate.Date,
            RATE = rate.Rate
        };
    }

    public static explicit operator ExchangeRate(ExchangeRateRow row)
    {
        return new ExchangeRate(row.FROM ?? string.Empty, row.TO ?? string.Empty, row.DATE ?? default, row.RATE ?? 0);
    }
}