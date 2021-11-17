using Flow.Domain.ExchangeRates;
using Flow.Domain.ExchangeRates.Validation;
using FluentValidation;

namespace Flow.Application.ExchangeRates.Validation;

internal class ExchangeRateValidator : AbstractValidator<ExchangeRate>
{
    private readonly Func<ExchangeRate, bool> rateIsPositive = ExchangeRatesValidationRules.RateIsPositive.Compile();
    public ExchangeRateValidator(IValidator<ExchangeRateRequest> requestValidator)
    {
        RuleFor(r => r).Custom((r, c) =>
        {
            var result = requestValidator.Validate(r);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    c.AddFailure(error.PropertyName, error.ErrorMessage);
                }
            }

            if (!rateIsPositive(r)) { c.AddFailure(nameof(r.Rate), "Rate cannot be zero or negative!"); }
        });
    }
}