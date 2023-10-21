namespace Flow.Infrastructure.IO.Contract;

public interface IFormatSpecificWriter<T> : IFormatSpecific
{
    Task Write(StreamWriter writer, IEnumerable<T> items, CancellationToken ct);
}
