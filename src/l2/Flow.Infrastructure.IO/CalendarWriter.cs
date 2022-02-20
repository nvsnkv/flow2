using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

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
        switch (format)
        {
            case SupportedFormat.CSV:
                await csv.Write(writer, calendar, ct);
                return;

            case SupportedFormat.JSON:
                await json.Write(writer, calendar, ct);
                return;

            default:
                throw new NotSupportedException($"Format {format} is not supported!");
        }
    }
}