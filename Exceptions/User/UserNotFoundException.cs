
public class UserNotFoundException : DomainExceptionBase
{
    public string UserId { get; }

    public UserNotFoundException(string userId) : base("User", $"User with ID: '{userId}' could not be found.")
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