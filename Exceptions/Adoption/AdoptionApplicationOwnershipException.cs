
public class AdoptionApplicationOwnershipException : DomainExceptionBase
{
    public int AdoptionApplicationId { get; }
    public string UserId { get; }

    public AdoptionApplicationOwnershipException(int adoptionApplicationId, string userId)
        : base("Adoption", $"User with ID: '{userId}' does not have permission to access adoption application with ID: '{adoptionApplicationId}'.")
    {
        AdoptionApplicationId = adoptionApplicationId;
        UserId = userId;
    }

    public override Dictionary<string, object> GetContextData()
    {
        var context = base.GetContextData();
        context["AdoptionApplicationId"] = AdoptionApplicationId;
        context["UserId"] = UserId;
        return context;
    }
}