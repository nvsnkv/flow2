using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.Entrypoint.Commands;

internal class CommandBase
{
    [Value(0, MetaName = "Args", Required = true, HelpText = "Sub-command sequence")]
    public IEnumerable<string> Agrs { get; [UsedImplicitly] set; }
}