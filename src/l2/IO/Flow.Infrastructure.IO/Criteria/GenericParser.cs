using System.Globalization;

namespace Flow.Infrastructure.IO.Criteria;

internal class GenericParser
{
    private readonly CultureInfo culture;
    private readonly NumberStyles numberStyles;
    private readonly DateTimeStyles dateTimeStyles;

    public GenericParser(CultureInfo culture, DateTimeStyles dateTimeStyles, NumberStyles numberStyles)
    {
        this.dateTimeStyles = dateTimeStyles;
        this.numberStyles = numberStyles;
        this.culture = culture;
    }

    public bool TryParseCollection<T>(string value, out List<T> list)
    {
        var result = new List<T>();
        var converted = value.Split(',').Select(p =>
        {
            if (TryParseSingleValue<T>(p, out var v))
            {
                result.Add(v!);
                return true;
            }

            return false;
        });

        list = result;
        return converted.All(c => c);
    }

    public bool TryParseBoundaries<T>(string value, out T min, out T max)
    {
        min = max = default!;
        var split = value.Split(":");
        if (split.Length != 2) { return false; }

        return TryParseSingleValue(split[0], out min!) && TryParseSingleValue(split[1], out max!);
    }

    public bool TryParseSingleValue<T>(string arg, out T? o)
    {
        var result = false;
        o = default;

        var type = typeof(T);
        if (type == typeof(decimal))
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (result = decimal.TryParse(arg, numberStyles, culture, out var d))
            {
                o = (T)(object)d;
            }
        }

        if (type == typeof(long))
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (result = long.TryParse(arg, numberStyles, culture, out var l))
            {
                o = (T)(object)l;
            }
        }

        if (type == typeof(DateTime))
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (result = DateTime.TryParse(arg, culture, dateTimeStyles, out var d))
            {
                o = (T)(object)d.ToUniversalTime();
            }
        }

        if (type == typeof(string))
        {
            o = (T)(object)arg;
            result = true;
        }

        return result;
    }
}