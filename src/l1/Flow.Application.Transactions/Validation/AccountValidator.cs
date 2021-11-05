using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Validation;
using FluentValidation;
using FluentValidation.Results;

namespace Flow.Application.Transactions.Validation;

internal class AccountValidator : AbstractValidator<AccountInfo>
{
    private readonly Func<AccountInfo, bool> nameIsNotEmpty = AccountInfoValidationRules.NameIsNotEmpty.Compile();
    private readonly Func<AccountInfo, bool> bankIsNotEmpty = AccountInfoValidationRules.BankIsNotEmpty.Compile();
    AccountValidator()
    {
        
        RuleFor(a => a).Custom((a, c) =>
        {
            if (!nameIsNotEmpty(a)) { c.AddFailure(nameof(a.Name), "Account name can not be empty!"); }
            if (!bankIsNotEmpty(a)) { c.AddFailure(nameof(a.Bank), "Bank name can not be empty!"); }
        });
    }
}