using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

internal class ArgsBase
{
    protected SupportedFormat? GuessFormatFromPath(string? input)
    {
        var ext = Path.GetExtension(input)?.ToLower();
        return ext switch
        {
            "csv" => SupportedFormat.CSV,
            "json" => SupportedFormat.JSON,
            null => null,
            _ => throw new NotSupportedException("File type is not supported!")
        };
    }
}