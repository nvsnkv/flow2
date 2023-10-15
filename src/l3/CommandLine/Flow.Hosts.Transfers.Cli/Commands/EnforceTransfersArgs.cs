using CommandLine;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transfers.Cli.Commands;

[Verb("enforce", HelpText = "Enforces transfers.")]
internal class EnforceTransfersArgs : ArgsBase
{
    private string? input;

    [Option('i', "input-file", Required = false, HelpText = "Input file path. If specified, app will use the file instead of standard input.")]
    public string? Input
    {
        get => input;
        [UsedImplicitly]
        set
        {
            input = value;
            Format = GuessFormatFromPath(value) ?? Format;
        }
    }

    [Option('f', "input-format", Required = false, Default = OldSupportedFormat.CSV, HelpText = "Input format.If input-file is set, input format will be defined by extension of the file and this option will be ignored.")]
    public OldSupportedFormat Format { get; [UsedImplicitly] set; }

    [Option("output-errors", Required = false, HelpText = "Errors file path. If specified, app will write list of rejected transfers to this file, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Errors { get; [UsedImplicitly] set; }

}