using Flow.Domain.Analysis;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.Csv;
using Flow.Infrastructure.IO.Json;

namespace Flow.Infrastructure.IO;

internal class FlowWriter : IFlowWriter
{
    private readonly CsvSerializer csv;
    private readonly JsonSerializer json;

    public FlowWriter(CsvSerializer csv, JsonSerializer json)
    {
        this.csv = csv;
        this.json = json;
    }

    public async Task Write(StreamWriter writer, IAsyncEnumerable<FlowItem> items, SupportedFormat format, CancellationToken ct)
    {
        switch (format) 
        {
            case SupportedFormat.CSV:  
                await csv.Write<FlowItem, FlowRow, FlowRowMap>(writer, items, i => (FlowRow)i, ct);
                break;

            case SupportedFormat.JSON: 
                 await json.Write(writer, items, ct);
                 break;

            default: 
                throw new NotSupportedException($"Format {format} is not supported!");
        };
    }
}