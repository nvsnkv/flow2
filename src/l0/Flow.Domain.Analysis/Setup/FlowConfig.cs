using Flow.Domain.Transactions;
using System.Linq.Expressions;

namespace Flow.Domain.Analysis.Setup;

public record FlowConfig(
    DateTime From,
    DateTime Till,
    string Currency,
    Expression<Func<RecordedTransaction, bool>>? Filter = null
    );