namespace Flow.Infrastructure.IO.Contract;

public interface IFormatSpecificWriter<T> : IFormatSpecific<T>
{
    Task Write(StreamWriter writer, IEnumerable<T> items, CancellationToken ct);
}
