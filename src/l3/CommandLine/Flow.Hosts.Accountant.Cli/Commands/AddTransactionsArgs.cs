using CommandLine;
using Flow.Infrastructure.Configuration.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Accountant.Cli.Commands;

[Verb("add", HelpText = "Add new transactions to storage."), UsedImplicitly]
internal class AddTransactionsArgs : ArgsBase
{
    private string? input;

    [Option('f', "input-format", Required = false, Default = SupportedFormat.CSV, HelpText = "Input format. If specified, app will use it to parse incoming data. If not specified, app will use file extension to define input format. This parameter is required if input-file is not set.")]
    public SupportedFormat Format { get; [UsedImplicitly] set; }

    [Option('i', "input-file", Required = false, HelpText = "Input file path. If specified, app will use the file instead of standard input.")]
    public string? Input
    {
        get => input;
        [UsedImplicitly] set
        {
            input = value;
            Format = GuessFormatFromPath(value) ?? Format;
        }
    }

    [Option('o', "output-file", Required = false, HelpText = "Output file path. If specified, app will write list of rejected transactions to this file, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Output { get; [UsedImplicitly] set; }
}