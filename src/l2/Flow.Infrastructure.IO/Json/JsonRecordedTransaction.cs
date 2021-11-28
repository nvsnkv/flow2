using Flow.Domain.Transactions;
using JetBrains.Annotations;

namespace Flow.Infrastructure.IO.Json;

internal class JsonRecordedTransaction : JsonTransaction
{
    public long? Key { get; set; }

    public JsonOverride? Overrides { get; set; }

    [UsedImplicitly]
    public class JsonOverride
    {
        public string? Comment { get; set; }

        public string? Category { get; set; }

        public string? Title { get; set; }
    }

    public static explicit operator RecordedTransaction(JsonRecordedTransaction js)
    {
        var transaction = (Transaction)(JsonTransaction)js;
        var result = new RecordedTransaction(js.Key ?? default, transaction);

        if (!string.IsNullOrEmpty(js.Overrides?.Comment) ||
            !string.IsNullOrEmpty(js.Overrides?.Category) ||
            !string.IsNullOrEmpty(js.Overrides?.Title))
        {
            result.Overrides = new Overrides(js.Overrides?.Category, js.Overrides?.Title, js.Overrides?.Comment);
        }

        return result;
    }
}