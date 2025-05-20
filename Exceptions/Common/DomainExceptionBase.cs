
public abstract class DomainExceptionBase : Exception
{
    public string Domain { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }

    public DomainExceptionBase(string domain, string message) : base(message)
    {
        Domain = domain;
        OccurredAt = DateTime.UtcNow;
        CorrelationId = Guid.NewGuid().ToString();
    }

    public virtual Dictionary<string, object> GetContextData()
    {
        return new Dictionary<string, object>
        {
            ["Domain"] = Domain,
            ["OccurredAt"] = OccurredAt,
            ["CorrelationId"] = CorrelationId
        };
    }
}