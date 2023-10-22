using CommandLine;

namespace Flow.Hosts.Transactions.Import.Cli.Commands;

[Verb("start", true, HelpText = "Starts new import session and appends all transactions found in a working directory")]
internal class StartCommandArgs : ImportCommandArgsBase { }