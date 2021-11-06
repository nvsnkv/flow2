using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Validation;
using FluentValidation;

namespace Flow.Application.Transactions.Validation;

internal class TransactionValidator : AbstractValidator<Transaction>
{
    private readonly Func<Transaction, bool> tsNotEmpty = TransactionValidationRules.TimestampIsNotEmpty.Compile();
    private readonly Func<Transaction, bool> amountNotZero = TransactionValidationRules.AmountIsNotZero.Compile();
    private readonly Func<Transaction, bool> currencyNotEmpty = TransactionValidationRules.CurrencyIsNotEmpty.Compile();
    private readonly Func<Transaction, bool> titleNotEmpty = TransactionValidationRules.TitleIsNotEmpty.Compile();
    
    public TransactionValidator(IValidator<AccountInfo> accountValidator)
    {
        RuleFor(t => t).Custom((t, c) =>
        {
            var result = accountValidator.Validate(t.Account);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    c.AddFailure(error);
                }
            }

            if (!tsNotEmpty(t)) { c.AddFailure(nameof(t.Timestamp), "Timestamp can not be empty!");}
            if (!amountNotZero(t)) { c.AddFailure(nameof(t.Amount), "Amount cam not be 0!"); }
            if (!currencyNotEmpty(t)) { c.AddFailure(nameof(t.Currency), "Currency can not be empty!"); }
            if (!titleNotEmpty(t)) {c.AddFailure(nameof(t.Title), "Title cannot be empty!"); }
        });
    }
}