using CommandLine;
using Flow.Infrastructure.Configuration.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Accountant.Cli.Commands;

[Verb("list", true, HelpText = "Lists transaction stored in the database."), UsedImplicitly]
internal class ListTransactionsArgs : ArgsBase
{
    private string? output;

    [Option('f', "output-format", Default = SupportedFormat.CSV, HelpText = "Output format. If output-file is set, output format will be defined by extension of output-file and this option will be ignored.")]
    public SupportedFormat Format { get; set; }

    [Option('o', "output-file",
        HelpText =
            "Output file path. If specified, app will this path to write the list of transactions stored in the database, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Output
    {
        get => output;
        set 
        {
            output= value;
            Format = GuessFormatFromPath(value) ?? Format;
        }
    }

    [Value(0, MetaName = "Criteria", Required = false, HelpText = "Search criteria")]
    public IEnumerable<string>? Criteria { get; set; }
}