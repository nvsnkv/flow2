using CsvHelper.Configuration;

namespace Flow.Infrastructure.IO.Csv;

internal sealed class FlowRowMap : ClassMap<FlowRow>
{
    public FlowRowMap() 
    {
        Map(m => m.KEY).Index(0);
        Map(m => m.TIMESTAMP).Index(1);
        Map(m => m.AMOUNT).Index(2);
        Map(m => m.CURRENCY).Index(3);
        Map(m => m.CATEGORY).Index(4);
        Map(m => m.TITLE).Index(5);
        Map(m => m.TYPE).Index(6);
    }
}