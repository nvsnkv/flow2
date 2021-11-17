using Flow.Domain.ExchangeRates;
using Flow.Domain.ExchangeRates.Validation;
using FluentValidation;

namespace Flow.Application.ExchangeRates.Validation;

internal class ExchangeRateRequestValidator : AbstractValidator<ExchangeRateRequest>
{
    private readonly Func<ExchangeRateRequest, bool> fromIsNotEmpty = ExchangeRatesValidationRules.FromIsNotEmpty.Compile();
    private readonly Func<ExchangeRateRequest, bool> toIsNotEmpty = ExchangeRatesValidationRules.ToIsNotEmpty.Compile();
    private readonly Func<ExchangeRateRequest, bool> currenciesAreDifferent = ExchangeRatesValidationRules.CurrenciesAreDifferent.Compile();
    private readonly Func<ExchangeRateRequest, bool> dateIsNotDefault = ExchangeRatesValidationRules.DateIsNotDefault.Compile();
    public ExchangeRateRequestValidator()
    {
        RuleFor(r => r).Custom((r, c) =>
        {
            if (!fromIsNotEmpty(r)) { c.AddFailure(nameof(r.From), "Currency to convert from can not be empty!"); }
            if (!toIsNotEmpty(r)) { c.AddFailure(nameof(r.To), "Currency to convert to can not be empty!"); }
            if (!currenciesAreDifferent(r)) { c.AddFailure("Currencies must be different!"); }
            if (!dateIsNotDefault(r)) { c.AddFailure(nameof(r.Date),"Date cannot be default!"); }
        });
    }
}