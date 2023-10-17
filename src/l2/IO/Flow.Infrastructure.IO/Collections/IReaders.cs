using Flow.Infrastructure.IO.Contract;

namespace Flow.Infrastructure.IO.Collections;

public interface IReaders<T> : ICollectionOfFormatSpecificItems<IFormatSpecificReader<T>> { }
