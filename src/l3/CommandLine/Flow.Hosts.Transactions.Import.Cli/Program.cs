using System.Diagnostics.CodeAnalysis;
using Autofac;
using CommandLine;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Common;
using Flow.Hosts.Transactions.Import.Cli.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Import.Contract;
using Flow.Infrastructure.Plugins.Loader;
using Flow.Infrastructure.Rates.CBRF.Contract;
using Flow.Infrastructure.Storage.Contract;

var builder = new ContainerBuilder();
var flowConfigurationModule = new FlowConfiguration();

builder.RegisterModule(flowConfigurationModule);
var container = builder.Build();
var config = container.Resolve<IFlowConfiguration>();

builder = new ContainerBuilder();
builder.RegisterModule(flowConfigurationModule)
    .RegisterModule(new PluginsModule(config))
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIO(config))
    .RegisterModule(new ImportIO())
    .RegisterModule(new CBRFData())
    .RegisterModule(new MoneyExchange())
    .RegisterModule(new TransactionsManagement());

builder.RegisterType<StartCommandHandler>();
builder.RegisterType<CompleteCommandHandler>();
builder.RegisterType<AbortCommandHandler>();
builder.RegisterType<EditCommandHandler>();

container = builder.Build();

var parser = ParserHelper.Create(config);
var cancellationHandler = new ConsoleCancellationHandler();

var arguments = parser.ParseArguments<StartCommandArgs, EditCommandArgs, CompleteCommandArgs, AbortCommandArgs>(args);

return await arguments.MapResult(
    async (StartCommandArgs args) => await container.Resolve<StartCommandHandler>().Start(args, cancellationHandler.Token),
    async (EditCommandArgs args) => await container.Resolve<EditCommandHandler>().Edit(args, cancellationHandler.Token),
    async (CompleteCommandArgs args) => await container.Resolve<CompleteCommandHandler>().Complete(args, cancellationHandler.Token),
    async (AbortCommandArgs args) => await container.Resolve<AbortCommandHandler>().Abort(args, cancellationHandler.Token),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));
