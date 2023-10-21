using System.Globalization;
using Autofac;
using CsvHelper.Configuration;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Criteria.Contract;
using Newtonsoft.Json;
using JsonSerializer = Flow.Infrastructure.IO.Calendar.JsonSerializer;

namespace Flow.Infrastructure.IO.Calendar.Contract;

public class CalendarIO : Module
{
    public static readonly ISupportedFormats Formats = new SupportedFormats();

    private readonly CultureInfo culture;

    public CalendarIO(CultureInfo culture)
    {
        this.culture = culture;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterInstance(new CalendarWriter(
                new CsvCalendarWriter(new CsvConfiguration(culture) { HeaderValidated = null }),
                new JsonCalendarWriter(new JsonSerializerSettings() { Culture = culture, Formatting = Formatting.Indented })
            )
        ).AsImplementedInterfaces();

        builder.Register(c =>
        {
            var json = new JsonSerializer(new JsonSerializerSettings() { Culture = culture, Formatting = Formatting.Indented });
            var criteriaParser = c.Resolve<ITransactionCriteriaParser>();

            return new JsonCalendarConfigParser(json, criteriaParser);
        }).InstancePerLifetimeScope().AsImplementedInterfaces();
        base.Load(builder);
    }

    public interface ISupportedFormats
    {
        SupportedFormat CSV { get; }

        SupportedFormat JSON { get; }
    }

    private class SupportedFormats : ISupportedFormats
    {
        public SupportedFormat CSV { get; } = new("CSV");
        public SupportedFormat JSON { get; } = new("JSON");
    }
}
