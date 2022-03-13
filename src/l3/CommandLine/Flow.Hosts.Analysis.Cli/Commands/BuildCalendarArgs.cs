using CommandLine;
using JetBrains.Annotations;

namespace Flow.Hosts.Analysis.Cli.Commands;

[Verb("calendar", HelpText = "Builds calendar-like aggregation of transactions grouped by dimensions defined by user."), UsedImplicitly]
internal class BuildCalendarArgs : BuildFlowArgs
{
    [Option('s', "input-series", Required = false, HelpText = "File with series setup. If specified, app will use this file to build calendar series, otherwise app will use standard input.")]
    public string? SeriesSetup { get; [UsedImplicitly] set; }

    [Option('d', "depth", Required = false, HelpText = "Aggregation depth. If specified, app will export series which depth is less or equal to value provided.")]
    public int? Depth { get; [UsedImplicitly] set; }
}
