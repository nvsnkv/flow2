using CommandLine;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Cli.Commands;

[Verb("list", true, HelpText = "Lists transaction stored in the database."), UsedImplicitly]
internal class ListTransactionsArgs : ArgsBase
{
    private string? output;

    [Option('f', "output-format", Default = ArgsBase.CSV, HelpText = "Output format. If output-file is set, output format will be defined by extension of output-file and this option will be ignored.")]
    public SupportedFormat Format { get; [UsedImplicitly] set; } = new(ArgsBase.CSV);

    [Option('o', "output-file", HelpText = "Output file path. If specified, app will this path to write the list of transactions stored in the database, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Output
    {
        get => output;
        [UsedImplicitly] set
        {
            output= value;
            Format = GuessFormatFromPath(value) ?? Format;
        }
    }

    [Option('e', "open-in-editor", Default = false, Required = false, HelpText = "Display transactions in external editor")]
    public bool OpenEditor { get; [UsedImplicitly] set;}

    [Option('d', "duplicates", HelpText = "Export the list of potentially duplicated entries instead of full list", Default = false)]
    public bool DuplicatesOnly { get; [UsedImplicitly] internal set; }

    [Option('r', "duplicates-range", HelpText = "Number of days between earliest and latest duplicates", Default = 3)]
    public int DuplicatesDaysRange { get; [UsedImplicitly] internal set; }

    [Value(0, MetaName = "Criteria", Required = true, HelpText = "Criteria for items that should be returned.")]
    public IEnumerable<string>? Criteria { get; [UsedImplicitly]  set; }
}
