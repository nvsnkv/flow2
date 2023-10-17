using CommandLine;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Cli.Commands;

[Verb("edit", HelpText = "Edit stored transactions using external editor."), UsedImplicitly]
internal class EditTransactionsArgs
{
    [Value(0, MetaName = "Criteria", Required = false, HelpText = "Criteria for items that needs to be updated.")]
    public IEnumerable<string>? Criteria { get; [UsedImplicitly] set; }

    [Option('f', "format", Default = "csv", HelpText = "Processing format. App will use this format to export data for update.")]
    public SupportedFormat Format { get; [UsedImplicitly] set; }

    [Option('e', "output-errors", Required = false, HelpText = "Output file path. If specified, app will write list of rejected transactions to this file, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Output { get; [UsedImplicitly] set; }
}
