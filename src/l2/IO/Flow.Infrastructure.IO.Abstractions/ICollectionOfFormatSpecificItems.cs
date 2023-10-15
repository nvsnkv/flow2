namespace Flow.Infrastructure.IO.Abstractions;

public interface ICollectionOfFormatSpecificItems<out T> where T : IFormatSpecific
{
    T GetFor(SupportedFormat format);

    public IEnumerable<SupportedFormat> GetKnownFormats();
}
