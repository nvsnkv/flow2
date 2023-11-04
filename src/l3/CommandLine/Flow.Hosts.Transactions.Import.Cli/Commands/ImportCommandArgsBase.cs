using CommandLine;
using Flow.Hosts.Common.Commands;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

internal abstract class ImportCommandArgsBase : ArgsBase
{
    [Option('d', "working-directory", Required = false, HelpText = "Working directory. Default is current directory")]
    public string WorkingDirectory { get; [UsedImplicitly] set; } = Environment.CurrentDirectory;
}
