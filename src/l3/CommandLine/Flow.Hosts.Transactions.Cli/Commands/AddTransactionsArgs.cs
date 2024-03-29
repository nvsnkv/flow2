﻿using CommandLine;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using JetBrains.Annotations;

namespace Flow.Hosts.Transactions.Cli.Commands;

[Verb("add", HelpText = "Add new transactions to storage."), UsedImplicitly]
internal class AddTransactionsArgs : ArgsBase
{
    private string? input;

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

    [Option('f', "input-format", Required = false,
        HelpText = "Input format.If input-file is set, input format will be defined by extension of the file and this option will be ignored.")]
    public SupportedFormat Format { get; [UsedImplicitly] set; } = new(ArgsBase.CSV);

    [Option('e', "edit-in-editor", Required = false, Default = false, HelpText = "Use external editor to update successfully appended transactions.")]
    public bool EditInEditor { get; [UsedImplicitly] set; }

    [Option("output-errors", Required = false, HelpText = "Errors file path. If specified, app will write list of rejected transactions to this file, otherwise it will either generate a new file or use standard output depending on configuration.")]
    public string? Errors { get; [UsedImplicitly] set; }
}
