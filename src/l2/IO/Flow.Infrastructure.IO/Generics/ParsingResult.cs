namespace Flow.Infrastructure.IO.Generics;

internal class ParsingResult
{
    protected ParsingResult(bool successful, string error)
    {
        Error = error;
        Successful = successful;
    }

    public ParsingResult(string error) : this(false, error)
    {
        
    }

    public string Error { get; }

    public virtual bool Successful { get; }
};
