namespace Flow.Infrastructure.IO.Contract;

public interface IFormatSpecificReader<T> : IFormatSpecific<T>
{
    Task<IEnumerable<T>> Read(StreamReader reader, CancellationToken ct);
}
