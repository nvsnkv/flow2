using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Calendar.Contract;

public interface ICalendarWriter
{
    Task WriteCalendar(StreamWriter writer, Domain.Analysis.Calendar calendar, SupportedFormat format, CancellationToken ct);
}
