using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.EntryPoint.Cli.Commands;

[Verb("exit", false, new []{"q", "quit" }, Hidden = true), UsedImplicitly]
internal sealed class ExitCommand
{
}