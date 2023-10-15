namespace Flow.Infrastructure.IO.Abstractions;

public interface IFormatSpecificWriter<T> : IFormatSpecific
{
    Task Write(StreamWriter writer, CancellationToken ct);
}