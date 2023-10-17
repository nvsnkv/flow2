using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Collections;

public interface ICollectionOfFormatSpecificItems<out T> where T : IFormatSpecific
{
    T GetFor(SupportedFormat format);

    public IEnumerable<SupportedFormat> GetKnownFormats();
}
