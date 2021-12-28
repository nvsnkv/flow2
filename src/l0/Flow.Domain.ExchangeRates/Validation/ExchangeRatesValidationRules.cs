using System.Linq.Expressions;

namespace Flow.Domain.ExchangeRates.Validation;

public static class ExchangeRatesValidationRules
{
    public static readonly Expression<Func<ExchangeRateRequest, bool>> FromIsNotEmpty = r => string.IsNullOrEmpty(r.From);

    public static readonly Expression<Func<ExchangeRateRequest, bool>> ToIsNotEmpty = r => string.IsNullOrEmpty(r.To);

    public static readonly Expression<Func<ExchangeRateRequest, bool>> CurrenciesAreDifferent = r => r.From != r.To;

    public static readonly Expression<Func<ExchangeRateRequest, bool>> DateIsNotDefault = r => r.Date != DateTime.MinValue;

    public static readonly Expression<Func<ExchangeRate, bool>> RateIsPositive = r => r.Rate > 0;
}