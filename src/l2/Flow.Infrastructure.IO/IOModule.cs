using System.Globalization;
using Autofac;
using CsvHelper.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO;

public class IOModule : Module
{
    private readonly IFlowConfiguration config;

    public IOModule(IFlowConfiguration config)
    {
        this.config = config;
    }

    protected override void Load(ContainerBuilder builder)
    {
        var culture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                          .FirstOrDefault(c => c.Name == config.CultureCode)
                      ?? CultureInfo.CurrentCulture;

        var csv = new CsvTransactionsSerializer(new CsvConfiguration(culture));
        var json = new JsonTransactionsSerializer(new JsonSerializerSettings() { Culture = culture });

        builder.Register(c => new TransactionsIOFacade(csv, json)).InstancePerLifetimeScope().AsImplementedInterfaces();

        base.Load(builder);
    }
}