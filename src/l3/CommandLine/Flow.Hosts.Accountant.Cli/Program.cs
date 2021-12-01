using Autofac;
using CommandLine;
using Flow.Application.ExchangeRates.Contract;
using Flow.Application.Transactions.Contract;
using Flow.Hosts.Accountant.Cli.Commands;
using Flow.Hosts.Common;
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
    

builder.RegisterType<EditTransactionsCommand>();
builder.RegisterType<ListTransactionsCommand>();
builder.RegisterType<DeleteTransactionsCommand>();

var container = builder.Build();
var config = container.Resolve<IFlowConfiguration>();

var parser = ParserHelper.Create(config);

var arguments = parser.ParseArguments<AddTransactionsArgs, ListTransactionsArgs, UpdateTransactionsArgs, EditTransactionsArgs, DeleteTransactionsArgs>(args);
return await arguments.MapResult(
    async (AddTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (ListTransactionsArgs arg) => await container.Resolve<ListTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (UpdateTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (EditTransactionsArgs arg) => await container.Resolve<EditTransactionsCommand>().Execute(arg, CancellationToken.None),
    async (DeleteTransactionsArgs arg) => await container.Resolve<DeleteTransactionsCommand>().Execute(arg, CancellationToken.None),
    async errs => await ParserHelper.HandleUnparsed(errs, arguments));