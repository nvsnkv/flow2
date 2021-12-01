using Autofac;
using CommandLine;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Common;
using Flow.Hosts.Transfers.Cli.Commands;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.Rates.CBRF.Contract;
using Flow.Infrastructure.Storage.Contract;

var builder = new ContainerBuilder();

builder.RegisterModule(new FlowConfiguration())
    .RegisterModule(new FlowDatabase())
    .RegisterModule(new FlowIOComponents())
    .RegisterModule(new CBRFData())
    .RegisterModule(new MoneyExchange())
    .RegisterModule(new TransactionsManagement());


builder.RegisterType<ListTransfersCommand>();
builder.RegisterType<EditTransfersCommand>();

var container = builder.Build();
var config = container.Resolve<IFlowConfiguration>();

var parser = ParserHelper.Create(config);

var arguments = parser.ParseArguments<ListTransfersArgs, EnforceTransfersArgs, AbandonTransfersArgs>(args);
return await arguments.MapResult(
    async (ListTransfersArgs arg) => await container.Resolve<ListTransfersCommand>().Execute(arg, CancellationToken.None),
    async (EnforceTransfersArgs arg) => await container.Resolve<EditTransfersCommand>().Execute(arg, CancellationToken.None),
    async (AbandonTransfersArgs arg) => await container.Resolve<EditTransfersCommand>().Execute(arg, CancellationToken.None),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));