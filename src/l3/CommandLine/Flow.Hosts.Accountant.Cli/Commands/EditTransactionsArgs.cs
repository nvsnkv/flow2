using CommandLine;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Hosts.Accountant.Cli.Commands;

[Verb("edit", HelpText = "Edit stored transactions using external editor.")]
internal class EditTransactionsArgs
{
    [Value(0, MetaName = "Criteria", Required = false, HelpText = "Search criteria")]
    public IEnumerable<string>? Criteria { get; set; }

    [Option('f', "output-format", Default = SupportedFormat.CSV, HelpText = "Output format. If output-file is set, output format will be defined by extension of output-file and this option will be ignored.")]
    public SupportedFormat Format { get; set; }

    [Option('o', "output-file", Required = false, HelpText = "Output file path. If specified, app will write list of rejected transactions to this file, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Output { get; set; }
}