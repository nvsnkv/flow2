using Flow.Domain.Transactions;
using FluentValidation;

namespace Flow.Application.Transactions.Validation;

internal class RecordedTransactionValidator : AbstractValidator<RecordedTransaction>
{
    private readonly IValidator<Transaction> transactionValidator;

    public RecordedTransactionValidator(IValidator<Transaction> transactionValidator)
    {
        this.transactionValidator = transactionValidator;
        RuleFor(t => t).Custom((r, c) =>
        {
            var result = transactionValidator.Validate(r);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    c.AddFailure(error);
                }
            }
        });
    }
}