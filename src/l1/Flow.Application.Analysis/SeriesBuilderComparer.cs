namespace Flow.Application.Analysis;

internal class SeriesBuilderComparer : IComparer<SeriesBuilder>
{
    private readonly VectorComparer vectorComparer;

    public SeriesBuilderComparer(VectorComparer vectorComparer)
    {
        this.vectorComparer = vectorComparer;
    }

    public int Compare(SeriesBuilder? x, SeriesBuilder? y) => vectorComparer.Compare(x?.Config.Measurement, y?.Config.Measurement);
}