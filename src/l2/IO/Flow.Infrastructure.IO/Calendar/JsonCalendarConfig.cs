using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.Calendar;

[UsedImplicitly]
internal class JsonCalendarConfig
{
    public List<string>? Dimensions { get; [UsedImplicitly] set; }

    public List<JsonSeriesConfig>? Series { get; [UsedImplicitly] set; }
}