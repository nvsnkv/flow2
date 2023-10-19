namespace Flow.Infrastructure.IO.Contract;

public interface IFormatSpecific<T>
{
    SupportedFormat Format { get; }
}
