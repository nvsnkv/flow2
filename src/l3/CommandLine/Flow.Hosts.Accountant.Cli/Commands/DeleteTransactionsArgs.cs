using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.Accountant.Cli.Commands;

[Verb("delete", HelpText = "Remove transactions from the storage."), UsedImplicitly]
internal class DeleteTransactionsArgs : ArgsBase
{
    [Value(0, MetaName = "Criteria", Required = true, HelpText = "Criteria for items that needs to be removed.")]
    public IEnumerable<string>? Criteria { get; [UsedImplicitly] set; }
}