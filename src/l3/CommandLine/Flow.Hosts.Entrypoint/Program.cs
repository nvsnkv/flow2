using System.Diagnostics;
using Autofac;
using Flow.Hosts.Common;
using Flow.Infrastructure.Configuration.Contract;
using System.Globalization;
using CommandLine;

var builder = new ContainerBuilder();
builder.RegisterModule(new FlowConfiguration());
var configContainer = builder.Build();

var config = configContainer.Resolve<IFlowConfiguration>();
var culture = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.Name == config.CultureCode) ?? CultureInfo.CurrentCulture;

var cancellationHandler = new ConsoleCancellationHandler();
var executor = new Executor(cancellationHandler.Token);

var parser = ParserHelper.Create(culture);

Task<int> Process(IReadOnlyCollection<string> arguments)
{
    var result = parser.ParseArguments<CoreCommand, TxCommand, XfersCommand, RatesCommand, BundleCommand>(arguments);
    return result.MapResult(
        async (CoreCommand c) => await executor!.Execute("flow-core", c.Agrs),
        async (TxCommand c) => await executor!.Execute("flow-tx", c.Agrs),
        async (XfersCommand c) => await executor!.Execute("flow-xfers", c.Agrs),
        async (RatesCommand c) => await executor!.Execute("flow-rates", c.Agrs),
        async (BundleCommand c) => await executor!.Execute("flow-bundle", c.Agrs),
        async errs => await ParserHelper.HandleUnparsed(errs, result)
    );

}

bool isInteractive = args.Length == 0;

return await Process(args);