using Autofac;
using CommandLine;
using Flow.Hosts.Analysis.Cli;
using Flow.Hosts.Analysis.Cli.Commands;
using Flow.Hosts.Common;
using Flow.Hosts.Common.Commands;
using Flow.Infrastructure.Configuration;
using Flow.Infrastructure.Configuration.Contract;

var builder = new ContainerBuilder();

builder.RegisterModule(new FlowConfiguration());

var container = builder.Build();

var config = container.Resolve<IFlowConfiguration>();

var parser = ParserHelper.Create(config);

var arguments = parser.ParseArguments<BuildCalendarArgs, ArgsBase>(args);

return await arguments.MapResult(async (BuildCalendarArgs arg) => await container.Resolve<BuildCalendarCommand>().Execute(arg, CancellationToken.None),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));