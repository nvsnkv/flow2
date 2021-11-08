using System;
using System.Linq.Expressions;
using Flow.Domain.Patterns;

namespace Flow.Domain.Transactions.Validation;

public class TransactionValidationRules
{
    public static readonly Expression<Func<Transaction, bool>> TimestampIsNotEmpty = t => t.Timestamp != default;
    public static readonly Expression<Func<Transaction, bool>> AmountIsNotZero = t => t.Amount != default;
    public static readonly Expression<Func<Transaction, bool>> CurrencyIsNotEmpty = t => !string.IsNullOrEmpty(t.Currency);
    public static readonly Expression<Func<Transaction, bool>> TitleIsNotEmpty = t => !string.IsNullOrEmpty(t.Title);
        

    public static readonly Func<Transaction, bool> CheckTransaction = new PatternBuilder<Transaction>()
        .With(TimestampIsNotEmpty)
        .With(AmountIsNotZero)
        .With(CurrencyIsNotEmpty)
        .With(TitleIsNotEmpty)
        .With(t => t.Account, a => AccountInfoValidationRules.Check(a))
        .Build()
        .Compile();

    protected TransactionValidationRules() {}
}