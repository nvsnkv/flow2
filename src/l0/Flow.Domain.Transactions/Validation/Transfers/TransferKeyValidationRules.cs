using System;
using System.Linq.Expressions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Domain.Transactions.Validation.Transfers;

public static class TransferKeyValidationRules
{
    public static readonly Expression<Func<TransferKey, bool>> SourceNotEqualToSink = t => t.Source != t.Sink;
}