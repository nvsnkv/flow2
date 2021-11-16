using Flow.Domain.ExchangeRates;
using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;
using Newtonsoft.Json;

namespace Flow.Infrastructure.IO.Json;

internal class JsonRejectionsWriter : JsonTransactionsSerializerBase
{
    public JsonRejectionsWriter(JsonSerializerSettings? settings) : base(settings)
    {
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransaction> rejections, CancellationToken ct)
    {
        await Write(writer, rejections, ct);
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedTransferKey> rejections, CancellationToken ct)
    {
        await Write(writer, rejections, ct);
    }

    public async Task WriteRejections(StreamWriter writer, IEnumerable<RejectedRate> rejections, CancellationToken ct)
    {
        await Write(writer, rejections, ct);
    }
}