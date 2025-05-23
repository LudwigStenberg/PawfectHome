
public class PetNotFoundException : DomainExceptionBase
{
    public int PetId { get; }

    public PetNotFoundException(int petId) : base("Pet", $"Pet with ID: '{petId}' could not be found")
    {
        PetId = petId;
    }

    public override Dictionary<string, object> GetContextData()
    {
        var context = base.GetContextData();
        context["PetId"] = PetId;
        return context;
    }
}