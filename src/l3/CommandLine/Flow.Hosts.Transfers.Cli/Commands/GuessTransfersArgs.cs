using CommandLine;

namespace Flow.Hosts.Transfers.Cli.Commands;

[Verb("guess", HelpText = "Generates the list of possible transfers detected within selected transactions")]
internal class GuessTransfersArgs : ListArgsBase
{
}