using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Calendar.Contract;
using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Calendar;

internal class CalendarWriter : ICalendarWriter
{
    private readonly CsvCalendarWriter csv;
    private readonly JsonCalendarWriter json;

    public CalendarWriter(CsvCalendarWriter csv, JsonCalendarWriter json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task WriteCalendar(StreamWriter writer, Domain.Analysis.Calendar calendar, SupportedFormat format, CancellationToken ct)
    {
        if (format == CalendarIO.Formats.CSV)
        {
            await csv.Write(writer, calendar, ct);
        }
        else if (format == CalendarIO.Formats.JSON)
        {
            await json.Write(writer, calendar, ct);
        }
        else {
            throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}
