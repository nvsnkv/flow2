using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;
using Flow.Infrastructure.IO.JSON.Contract;

namespace Flow.Hosts.Common.Commands;

public class ArgsBase
{
    protected static SupportedFormat? GuessFormatFromPath(string? input)
    {
        var ext = Path.GetExtension(input)?.ToLower();
        return ext switch
        {
            ".csv" => CSVIO.SupportedFormat,
            ".json" => JSONIO.SupportedFormat,
            null => null,
            _ => throw new NotSupportedException("File type is not supported!")
        };
    }
}
