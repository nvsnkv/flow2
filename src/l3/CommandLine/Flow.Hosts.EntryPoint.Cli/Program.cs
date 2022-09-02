using System.Diagnostics;
using Autofac;
using Flow.Hosts.Common;
using Flow.Infrastructure.Configuration.Contract;
using System.Globalization;
using CommandLine;
using Flow.Hosts.EntryPoint.Cli;
using Flow.Hosts.EntryPoint.Cli.Commands;

var builder = new ContainerBuilder();
builder.RegisterModule(new FlowConfiguration());
var configContainer = builder.Build();

var config = configContainer.Resolve<IFlowConfiguration>();
var culture = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.Name == config.CultureCode) ?? CultureInfo.CurrentCulture;

var cancellationHandler = new ConsoleCancellationHandler();
var executor = new Executor(Console.Error);

var parser = ParserHelper.Create(culture);

bool shouldExit = false;

Task<int> Process(IReadOnlyCollection<string> arguments)
{
    var result = parser.ParseArguments<CoreCommand, TxCommand, XfersCommand, RatesCommand, BundleCommand, ExitCommand>(arguments);
    return result.MapResult(
        async (CoreCommand c) => await executor.Execute("flow-core", c.Agrs),
        async (TxCommand c) => await executor.Execute("flow-tx", c.Agrs),
        async (XfersCommand c) => await executor.Execute("flow-xfers", c.Agrs),
        async (RatesCommand c) => await executor.Execute("flow-rates", c.Agrs),
        async (BundleCommand c) => await executor.Execute("flow-bundle", c.Agrs),
        (ExitCommand _) => { shouldExit = true; return Task.FromResult(0); },
        async errs => await ParserHelper.HandleUnparsed(errs, result)
    );

}

bool isInteractive = args.Length == 0;
if (!isInteractive)
{
    return await Process(args);
}
else
{
    while (!cancellationHandler.Token.IsCancellationRequested && !shouldExit)
    {
        await Console.Out.WriteAsync("> ");
        var input = await Console.In.ReadLineAsync() ?? string.Empty;
        if (cancellationHandler.Token.IsCancellationRequested)
        {
            return 1;
        }

        var parms = input.Split(' ');
        await Process(parms);
        await Console.Out.WriteLineAsync();
    }
}

return 0;