using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Collections;

public interface ICollectionOfFormatSpecificItems<out T, TE> where T : IFormatSpecific
{
    T GetFor(SupportedFormat format);

    public IEnumerable<SupportedFormat> GetKnownFormats();
}
