using CommandLine;
using Flow.Hosts.Common.Commands;
using JetBrains.Annotations;

namespace Flow.Hosts.Import.Cli.Commands;

internal abstract class ImportCommandArgsBase : ArgsBase
{
    [Option('d', "working-directory", HelpText = "Working directory")]
    public string WorkingDirectory { get; [UsedImplicitly] set; } = Environment.CurrentDirectory;

    [Option('v', "verbose", Default = false, HelpText = "Verbose output")]
    public bool Verbose { get; [UsedImplicitly] set; }
}
