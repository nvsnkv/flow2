using CommandLine;
using Flow.Hosts.Common.Commands;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Cli.Commands;

[Verb("delete", HelpText = "Remove transactions from the storage."), UsedImplicitly]
internal class DeleteTransactionsArgs : ArgsBase
{
    [Value(0, MetaName = "Criteria", Required = true, HelpText = "Criteria for items that needs to be removed.")]
    public IEnumerable<string>? Criteria { get; [UsedImplicitly] set; }
}