using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.Analysis.Cli.Commands;

[Verb("calendar", HelpText = "Builds calendar-like aggregation of transactions grouped by dimensions defined by user."), UsedImplicitly]
internal class BuildCalendarArgs : BuildFlowArgs
{
    [Option('d', "input-dimensions", Required = false, HelpText = "File with dimensions setup. If specified, app will use it to build calendar dimensions, otherwise app will use standard input.")]
    public string? DimensionsSetup { get; [UsedImplicitly] set; }
}
