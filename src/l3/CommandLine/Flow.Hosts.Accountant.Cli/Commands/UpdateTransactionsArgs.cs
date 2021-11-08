using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.Accountant.Cli.Commands;

[Verb("update", HelpText = "Update transactions that already recorded."), UsedImplicitly]
internal class UpdateTransactionsArgs : AddTransactionsArgs
{
}