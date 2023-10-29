using CommandLine;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

[Verb("edit", HelpText = "Allows to edit transactions captured within an import session")]
internal class EditCommandArgs : ImportCommandArgsBase
{
    [Option('f', "format", Default = CSV, HelpText = "Processing format. App will use this format to export data for update.")]
    public SupportedFormat Format { get; [UsedImplicitly] set; } = new(CSV);

    [Option('e', "output-errors", Required = false, HelpText = "Output file path. If specified, app will write list of rejected transactions to this file, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? ErrorsOutput { get; [UsedImplicitly] set; }
}
