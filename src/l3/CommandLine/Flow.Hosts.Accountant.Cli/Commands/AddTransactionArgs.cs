using System.Data;
using CommandLine;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Accountant.Cli.Commands;

[Verb("add", false, HelpText = "Add new transactions to storage"), UsedImplicitly]
internal class AddTransactionArgs
{
    public AddTransactionArgs(string? input, SupportedFormat? format, string? output)
    {
        Input = input;
        Output = output;
        if (input == null && format == null)
        {
            throw new ArgumentNullException(nameof(format), "Format is required if input file is not set");
        }

        Format = format ?? GuessFormatFromPath(input);
    }

    private SupportedFormat GuessFormatFromPath(string? input)
    {
        var ext = Path.GetExtension(input)?.ToLower();
        return ext switch
        {
            "csv" => SupportedFormat.CSV,
            "json" => SupportedFormat.JSON,
            null => throw new ArgumentException("Unable to guess a format from a file without extension!"),
            _ => throw new NotSupportedException("File type is not supported")
        };
    }

    [Option('i',"input-file", Required = false, HelpText = "Input file path. If specified, app will use the file instead of standard input")]
    public string? Input { get; }

    [Option('f', "input-format", Required = false, Default = null, HelpText = "Input format. If specified, app will use it to parse incoming data. If not specified, app will use file extension to define input format. This parameter is required if input file is not set.")]
    public SupportedFormat Format { get; }

    [Option('o', "output-file", Required = false, HelpText = "Output file path. If specified, app will write list of rejected transactions to this file, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Output { get; }
}