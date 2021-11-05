namespace Flow.Infrastructure.Configuration.Contract;
public interface IFlowConfiguration
{
    public string? ConnectionString { get; }
    string? CultureCode { get; set; }
}