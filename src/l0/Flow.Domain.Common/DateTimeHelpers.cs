namespace Flow.Domain.Common;

public static class DateTimeHelpers
{
    /// <summary>
    /// Returns the day when it will be most likely processed. Banks does not complete transactions over the weekend, so transactions happened on Sat or Sun will be recorded as processed on Monday.
    /// </summary>
    /// <param name="timestamp">Original date</param>
    /// <returns>The beginning of the closest working day which is greater or equal to Date property of incoming timestamp.</returns>
    public static DateTime BusinessDate(this DateTime timestamp)
    {
        return timestamp.DayOfWeek switch
        {
            DayOfWeek.Sunday => timestamp.Date.AddDays(1),
            DayOfWeek.Saturday => timestamp.Date.AddDays(2),
            _ => timestamp.Date
        };
    }
}