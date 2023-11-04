using CommandLine;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

[Verb("abort", HelpText = "Starts new import session and appends all transactions found in a working directory")]
internal class AbortCommandArgs : ImportCommandArgsBase { }
