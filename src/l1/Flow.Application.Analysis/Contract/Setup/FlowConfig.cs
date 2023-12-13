using System.Linq.Expressions;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis.Contract.Setup;

public record FlowConfig(
    DateTime From,
    DateTime Till,
    string Currency,
    Expression<Func<RecordedTransaction, bool>>? Criteria = null
    );
