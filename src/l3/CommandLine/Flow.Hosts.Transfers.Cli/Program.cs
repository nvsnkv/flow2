using Autofac;
using CommandLine;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Common;
using Flow.Hosts.Transfers.Cli.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
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
    .RegisterModule(new FlowIOModule())
    .RegisterModule(new CBRFData())
    .RegisterModule(new MoneyExchange())
    .RegisterModule(new TransactionsManagement());


builder.RegisterType<ListTransfersCommand>();
builder.RegisterType<EditTransfersCommand>();

container = builder.Build();

var cancellationHandler = new ConsoleCancellationHandler();
var parser = ParserHelper.Create(config);

var arguments = parser.ParseArguments<ListTransfersArgs, EnforceTransfersArgs, AbandonTransfersArgs, GuessTransfersArgs>(args);
return await arguments.MapResult(
    async (ListTransfersArgs arg) => await container.Resolve<ListTransfersCommand>().Execute(arg, cancellationHandler.Token),
    async (GuessTransfersArgs arg) => await container.Resolve<ListTransfersCommand>().Execute(arg, cancellationHandler.Token),
    async (EnforceTransfersArgs arg) => await container.Resolve<EditTransfersCommand>().Execute(arg, cancellationHandler.Token),
    async (AbandonTransfersArgs arg) => await container.Resolve<EditTransfersCommand>().Execute(arg, cancellationHandler.Token),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));
