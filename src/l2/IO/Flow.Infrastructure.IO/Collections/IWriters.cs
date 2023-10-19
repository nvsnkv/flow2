using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Collections;

public interface IWriters<T> : ICollectionOfFormatSpecificItems<IFormatSpecificWriter<T>, T> { }
