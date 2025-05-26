public class UserIdRequiredException : DomainExceptionBase
{
    public UserIdRequiredException() : base("User", "A user ID is required to perform this operation")
    { }
    public override Dictionary<string, object> GetContextData()
    {
        return base.GetContextData();
    }
}