namespace Flow.Infrastructure.IO.Abstractions;

public interface IReaders<T> : ICollectionOfFormatSpecificItems<IFormatSpecificReader<T>> { }
