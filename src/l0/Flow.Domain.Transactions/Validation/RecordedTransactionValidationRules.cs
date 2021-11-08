using System;
using System.Linq.Expressions;
using Flow.Domain.Patterns;

namespace Flow.Domain.Transactions.Validation;

public class RecordedTransactionValidationRules : TransactionValidationRules
{
    public static readonly Expression<Func<RecordedTransaction, bool>> KeyIsNotZero = t => t.Key != default;

    public static readonly Func<RecordedTransaction, bool> CheckRecordedTransaction = new PatternBuilder<RecordedTransaction>()
        .With(KeyIsNotZero)
        .With(t => TransactionValidationRules.CheckTransaction(t))
        .Build()
        .Compile();

    private RecordedTransactionValidationRules() {}
}