using CommandLine;

namespace Flow.Hosts.Transfers.Cli.Commands;

[Verb("list", true, HelpText = "Generates list of transfers detected within selected transactions")]
internal class ListTransfersArgs : ListArgsBase
{
}