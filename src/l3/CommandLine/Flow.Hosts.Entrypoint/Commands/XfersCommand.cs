﻿using CommandLine;
using Flow.Hosts.Entrypoint.Commands;

[Verb("xfers", HelpText = "Invokes transfers management module. Please refer to `xfers help` output for details")]
internal sealed class XfersCommand : CommandBase { }