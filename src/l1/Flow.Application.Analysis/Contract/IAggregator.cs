﻿using Flow.Domain.Analysis;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis.Contract;

public interface IAggregator
{
    Task<(Calendar, IReadOnlyCollection<RejectedTransaction>)> GetCalendar(DateTime from, DateTime till, string currency, Vector header, IEnumerable<AggregationRule> dimensions, CancellationToken ct);
    
    Task<(IAsyncEnumerable<RecordedTransaction>, IEnumerable<RejectedTransaction>)> GetFlow(DateTime from, DateTime till, string currency, CancellationToken ct);
}