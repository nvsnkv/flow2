using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.EntryPoint.Cli.Commands;

internal class CommandWithOptionalArgs
{
    [Value(0, MetaName = "Args", Required = false, HelpText = "Sub-command sequence")]
    public IEnumerable<string> Agrs { get; [UsedImplicitly] set; }
}