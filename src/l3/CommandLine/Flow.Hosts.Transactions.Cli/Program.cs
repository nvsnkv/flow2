using Autofac;
using CommandLine;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Accountant.Cli.Commands;
using Flow.Hosts.Common;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.Plugins.Transactions.Loader;
using Flow.Infrastructure.Rates.CBRF.Contract;
using Flow.Infrastructure.Storage.Contract;

var builder = new ContainerBuilder();
var flowConfigurationModule = new FlowConfiguration();

builder.RegisterModule(flowConfigurationModule);
var container = builder.Build();
var config = container.Resolve<IFlowConfiguration>();

builder = new ContainerBuilder();
builder.RegisterModule(flowConfigurationModule)
    .RegisterModule(new TransactionsPluginsModule(config))
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIOComponents())
    .RegisterModule(new CBRFData())
    .RegisterModule(new MoneyExchange())
    .RegisterModule(new TransactionsManagement());

builder.RegisterType<EditTransactionsCommand>();
builder.RegisterType<ListTransactionsCommand>();
builder.RegisterType<DeleteTransactionsCommand>();

container = builder.Build();

var parser = ParserHelper.Create(config);
var cancellationHandler = new ConsoleCancellationHandler();

var arguments = parser.ParseArguments<AddTransactionsArgs, ListTransactionsArgs, UpdateTransactionsArgs, EditTransactionsArgs, DeleteTransactionsArgs>(args);
return await arguments.MapResult(
    async (AddTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, cancellationHandler.Token),
    async (ListTransactionsArgs arg) => await container.Resolve<ListTransactionsCommand>().Execute(arg, cancellationHandler.Token),
    async (UpdateTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, cancellationHandler.Token),
    async (EditTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, cancellationHandler.Token),
    async (DeleteTransactionsArgs arg) => await container.Resolve<DeleteTransactionsCommand>().Execute(arg, cancellationHandler.Token),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));