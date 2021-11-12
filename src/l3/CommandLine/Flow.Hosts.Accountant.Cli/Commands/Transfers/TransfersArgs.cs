using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.Accountant.Cli.Commands.Transfers;

[Verb("transfers", HelpText = "Set of commands to handle money transfers.")]
internal class TransfersArgs : ArgsBase
{

    [Value(0, MetaName = "Args", Required = false)]
    public IEnumerable<string>? Args { get; [UsedImplicitly] set; }
}