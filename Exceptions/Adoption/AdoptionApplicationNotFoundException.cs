
public class AdoptionApplicationNotFoundException : DomainExceptionBase
{
    public int AdoptionApplicationId { get; }

    public AdoptionApplicationNotFoundException(int adoptionApplicationId)
        : base("Adoption", $"Adoption Application with ID: '{adoptionApplicationId}' could not be found.")
    {
        AdoptionApplicationId = adoptionApplicationId;
    }

    public override Dictionary<string, object> GetContextData()
    {
        var context = base.GetContextData();
        context["AdoptionApplicationId"] = AdoptionApplicationId;
        return context;
    }
}