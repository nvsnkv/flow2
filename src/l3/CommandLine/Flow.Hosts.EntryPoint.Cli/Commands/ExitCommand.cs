using CommandLine;

namespace Flow.Hosts.EntryPoint.Cli.Commands;

[Verb("exit", false, new []{"q", "quit" }, Hidden = true)]
internal sealed class ExitCommand
{
}