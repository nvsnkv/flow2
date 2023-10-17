using CommandLine;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.ExchangeRates.Cli.Commands;

[Verb("request", HelpText = "Request an exchange rate from remote provider."), UsedImplicitly]
internal class RequestArgs : ArgsBase
{
    [Option('f', "from", Required = true, HelpText = "Currency to convert from.")]
    public string? From { get; [UsedImplicitly] set; }

    [Option('t', "to", Required = true, HelpText = "Currency to convert to.")]
    public string? To { get; [UsedImplicitly] set; }

    [Option('d', "date", Required = true, HelpText = "As of date.")]
    public DateTime Date { get; [UsedImplicitly] set; }

    [Option("output-format", Required = true, Default = "csv", HelpText = "Output format.")]
    public SupportedFormat Format { get; [UsedImplicitly] set; }
}
