namespace Flow.Infrastructure.IO.Contract;

public interface IFormatSpecificReader<T> : IFormatSpecific
{
    Task<IEnumerable<T>> Read(StreamReader reader, CancellationToken ct);
}
