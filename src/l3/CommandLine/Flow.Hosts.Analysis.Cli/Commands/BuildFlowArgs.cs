using CommandLine;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Analysis.Cli.Commands;

[Verb("list", HelpText = "Builds the list of meaningful transactions that changes amount of money within the system.")]
internal class BuildFlowArgs : ArgsBase
{
    private string? output;
    private SupportedFormat format;

    [Option('f', "from", Required = true, HelpText = "Left boundary of date range to aggregate. Transactions with date greater or equal to this value will be included to aggregation.")]
    public DateTime From { get; [UsedImplicitly] set; }

    [Option('t', "till", Required = true, HelpText = "Right boundary of date range to aggregate. Transactions with date lesser than this value will be included to aggregation.")]
    public DateTime Till { get; [UsedImplicitly] set; }

    [Option('c', "currency", Required = true, HelpText = "Target currency. Transactions in different currency will be converted to target currency for proper aggregation.")]
    public string Currency { get; [UsedImplicitly] set; } = null!;

    [Option("output-format", Default = SupportedFormat.CSV, HelpText = "Output format. If output-file is set, output format will be defined by extension of output-file and this option will be ignored.")]
    public SupportedFormat Format
    {
        get => format;
        [UsedImplicitly]
        set
        {
            if (!string.IsNullOrEmpty(output)) return;
            format = value;
        }
    }

    [Option('o', "output-file", Required = false, HelpText = "Output file path.If specified, app will this path to write the calendar, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? OutputPath
    {
        get => output;
        [UsedImplicitly]
        set
        {
            output = value;
            format = GuessFormatFromPath(value) ?? Format;
        }
    }
    
    [Option('r', "output-rejected", Required = false, HelpText = "Path to write rejected transactions. If specified, app will use this path to write the list of transactions excluded from aggregation, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? RejectedPath { get; [UsedImplicitly] set; }
    
    [Value(0, MetaName = "Criteria", Required = false, HelpText = "Criteria for items that should be returned.")]
    public IEnumerable<string>? Criteria { get; [UsedImplicitly] set; }
}