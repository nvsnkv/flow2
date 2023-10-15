namespace Flow.Infrastructure.IO.Abstractions;

public interface IWriters<T> : ICollectionOfFormatSpecificItems<IFormatSpecificWriter<T>> { }