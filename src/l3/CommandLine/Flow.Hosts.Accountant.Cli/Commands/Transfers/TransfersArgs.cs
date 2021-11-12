using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.Accountant.Cli.Commands.Transfers;

[Verb("transfers", HelpText = "Set of commands to handle money transfers."), UsedImplicitly]
internal class TransfersArgs : ArgsBase
{
}