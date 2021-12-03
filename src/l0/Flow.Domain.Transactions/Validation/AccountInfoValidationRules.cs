using System;
using System.Linq.Expressions;
using Flow.Domain.Patterns;

namespace Flow.Domain.Transactions.Validation;

public static class AccountInfoValidationRules
{
    public static readonly Expression<Func<AccountInfo, bool>> NameIsNotEmpty = a => !string.IsNullOrEmpty(a.Name);
    public static readonly Expression<Func<AccountInfo, bool>> BankIsNotEmpty = a => !string.IsNullOrEmpty(a.Bank);

    public static readonly Func<AccountInfo, bool> Check = new AndPatternBuilder<AccountInfo>().With(NameIsNotEmpty).With(BankIsNotEmpty).Build().Compile();
}