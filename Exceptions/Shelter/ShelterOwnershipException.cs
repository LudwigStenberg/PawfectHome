
public class ShelterOwnershipException : DomainExceptionBase
{
    public int ShelterId { get; }
    public string UserId { get; }

    public ShelterOwnershipException(int shelterId, string userId)
        : base("Shelter", $"User with ID: '{userId}' does not have permission to access shelter with ID: '{shelterId}'.")
    {
        ShelterId = shelterId;
        UserId = userId;
    }

    public override Dictionary<string, object> GetContextData()
    {
        var context = base.GetContextData();
        context["ShelterId"] = ShelterId;
        context["UserId"] = UserId;
        return context;
    }
}