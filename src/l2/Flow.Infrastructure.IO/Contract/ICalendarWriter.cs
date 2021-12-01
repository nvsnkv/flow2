using Flow.Domain.Analysis;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Infrastructure.IO.Contract;

public interface ICalendarWriter
{
    Task WriteCalendar(StreamWriter writer, Calendar calendar, SupportedFormat format, CancellationToken ct);
}