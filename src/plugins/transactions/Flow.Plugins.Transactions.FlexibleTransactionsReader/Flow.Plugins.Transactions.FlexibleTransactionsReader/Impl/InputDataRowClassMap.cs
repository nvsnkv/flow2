using System.Reflection;
using CsvHelper.Configuration;
using Flow.Plugins.Transactions.FlexibleTransactionsReader.Settings;

namespace Flow.Plugins.Transactions.FlexibleTransactionsReader.Impl;

internal sealed class InputDataRowClassMap : ClassMap<InputDataRow>
{
    public InputDataRowClassMap(IReadOnlyCollection<MappingRule>? rules)
    {
        var type = typeof(InputDataRow);
        var rulesDict = rules?
            .Where(r => r.Field != null)
            .ToDictionary(r => r.Field!)
            ?? new Dictionary<string, MappingRule>();

        foreach (var info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var map = Map(type, info).Optional();
            if (rulesDict.TryGetValue(info.Name, out var rule))
            {
                if (rule.ColumnId.HasValue)
                {
                    map = map.Index(rule.ColumnId.Value);
                }

                if (rule.FallbackValue != null)
                {
                    map.Default(rule.FallbackValue);
                }
            }
            else
            {
                map.Ignore();
            }
        }
    }
}
