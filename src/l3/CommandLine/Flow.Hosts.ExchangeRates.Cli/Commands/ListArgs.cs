using CommandLine;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.ExchangeRates.Cli.Commands;

[Verb("list", true, HelpText = "List exchange rates from storage.")]
internal class ListArgs : ArgsBase
{
    private string? output;

    [Option('f', "output-format", Default = "csv", HelpText = "Output format. If output-file is set, output format will be defined by extension of output-file and this option will be ignored.")]
    public SupportedFormat Format { get; [UsedImplicitly] set; }

    [Option('o', "output-file", HelpText = "Output file path. If specified, app will this path to write the list of exchange rates stored in the database, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Output
    {
        get => output;
        [UsedImplicitly]
        set
        {
            output = value;
            Format = GuessFormatFromPath(value) ?? Format;
        }
    }
}
