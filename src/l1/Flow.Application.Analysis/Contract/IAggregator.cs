using Flow.Domain.Analysis;
using Flow.Domain.Analysis.Setup;
using Flow.Domain.Transactions;

namespace Flow.Application.Analysis.Contract;

public interface IAggregator
{
    Task<(Calendar, IReadOnlyCollection<RejectedTransaction>)> GetCalendar(FlowConfig flowConfig, CalendarConfig calendarConfig, CancellationToken ct = default);
    
    Task<(IAsyncEnumerable<RecordedTransaction>, IEnumerable<RejectedTransaction>)> GetFlow(FlowConfig flowConfig, CancellationToken ct = default);
}