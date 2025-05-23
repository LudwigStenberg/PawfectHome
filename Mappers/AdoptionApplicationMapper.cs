
public static class AdoptionApplicationMapper
{
    public static AdoptionApplicationEntity ToEntity(RegisterAdoptionRequest request)
    {
        return new AdoptionApplicationEntity
        {
            UserId = request.UserId,
            PetId = request.PetId
        };
    }
}