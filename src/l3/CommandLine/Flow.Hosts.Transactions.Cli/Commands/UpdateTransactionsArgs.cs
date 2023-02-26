using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Cli.Commands;

[Verb("update", HelpText = "Update transactions that already recorded."), UsedImplicitly]
internal class UpdateTransactionsArgs : AddTransactionsArgs
{
}