using CommandLine;

namespace Flow.Hosts.Accountant.Cli.Commands;

[Verb("delete", HelpText = "Remove transactions from the storage.")]
internal class DeleteTransactionsArgs : ArgsBase
{
    [Value(0, MetaName = "Criteria", Required = true, HelpText = "Criteria for items that needs to be removed.")]
    public IEnumerable<string>? Criteria { get; set; }
}