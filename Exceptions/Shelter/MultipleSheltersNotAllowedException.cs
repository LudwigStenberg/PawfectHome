public class MultipleSheltersNotAllowedException : DomainExceptionBase
{
    public string UserId { get; }

    public MultipleSheltersNotAllowedException(string userId)
        : base("Shelter", $"User with ID: '{userId}' already has a shelter. Each user can only have one shelter registered at a time.")
    {
        UserId = userId;
    }

    public override Dictionary<string, object> GetContextData()
    {
        var context = base.GetContextData();
        context["UserId"] = UserId;
        return context;
    }
}