public class ShelterNotFoundException : DomainExceptionBase
{
    public int ShelterId { get; }

    public ShelterNotFoundException(int shelterId)
        : base("Shelter", $"Shelter with ID: '{shelterId}' could not be found.")
    {
        ShelterId = shelterId;
    }

    public override Dictionary<string, object> GetContextData()
    {
        var context = base.GetContextData();
        context["ShelterId"] = ShelterId;
        return context;
    }
}