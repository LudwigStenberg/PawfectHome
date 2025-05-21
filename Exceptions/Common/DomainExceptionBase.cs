
/// <summary>
/// Abstract class for domain-specific exceptions. Provides a common infrastructure for
///  business rule violations and captures structured context data. 
/// </summary>
/// /// <remarks>
/// Derived classes should override <see cref="GetContextData"/> to include domain-specific context.
/// </remarks>
public abstract class DomainExceptionBase : Exception
{
    public string Domain { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }

    /// <param name="domain">The business domain identifier.</param>
    /// <param name="message">User-appropriate error message.</param>
    public DomainExceptionBase(string domain, string message) : base(message)
    {
        Domain = domain;
        OccurredAt = DateTime.UtcNow;
        CorrelationId = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Gets structured context data for logging and debugging.
    /// Override to include domain-specific information.
    /// </summary>
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