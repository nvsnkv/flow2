namespace Flow.Infrastructure.IO.Abstractions;

public interface IFormatSpecificReader<T> : IFormatSpecific
{
    Task<IEnumerable<T>> Read(StreamReader reader, CancellationToken ct);
}