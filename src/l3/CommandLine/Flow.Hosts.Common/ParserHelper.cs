using System.Globalization;
using CommandLine;
using CommandLine.Text;
using Flow.Infrastructure.Configuration.Contract;

namespace Flow.Hosts.Common;

public static class ParserHelper
{
    public static Parser Create(IFlowConfiguration flowConfiguration)
    {
        return new Parser(settings => {
            settings.AutoHelp = true;
            settings.AutoVersion = true;
            settings.CaseInsensitiveEnumValues = true;
            settings.IgnoreUnknownArguments = true;
            settings.ParsingCulture =
                CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .FirstOrDefault(c => c.Name == flowConfiguration.CultureCode)
                ?? CultureInfo.CurrentCulture;

            settings.EnableDashDash = true;
        });
    }

    public static async Task<int> HandleUnparsed(IEnumerable<Error> errs, ParserResult<object> parserResult)
    {
        var width = Console.WindowWidth;
        var errors = errs as Error[] ?? errs.ToArray();
        var output = HelpText.AutoBuild(parserResult, h =>
        {
            h.MaximumDisplayWidth = width;
            return h;
        });

        await errors.Output().WriteLineAsync(output);
        return errors.IsHelp() || errors.IsVersion() ? 0 : 1;
    }
}