﻿using CommandLine;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transfers.Cli.Commands;

internal abstract class ListArgsBase : ArgsBase
{
    private string? output;

    [Option('f', "output-format", Default = CSV,
        HelpText = "Output format. If output-file is set, output format will be defined by extension of output-file and this option will be ignored.")]
    public SupportedFormat Format { get; [UsedImplicitly] set; } = new(CSV);

    [Option('o', "output-file", HelpText = "Output file path. If specified, app will this path to write the list of transfers, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Output
    {
        get => output;
        [UsedImplicitly]
        set
        {
            output = value;
            Format = GuessFormatFromPath(value) ?? Format;
        }
    }

    [Option('e', "open-in-edior", Default = false, Required = false, HelpText = "Display transfers in external editor")]
    public bool OpenEditor { get; [UsedImplicitly] set; }

    [Value(0, MetaName = "Criteria", Required = true, HelpText = "Criteria for transactions within which application should search for transfers.")]
    public IEnumerable<string>? Criteria { get; [UsedImplicitly]  set; }
}
