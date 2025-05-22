public static class ShelterMapper
{
    public static ShelterEntity ToEntity(RegisterShelterRequest request, string userId)
    {
        return new ShelterEntity
        {
            Name = request.Name,
            Description = request.Description ?? "No descrpiton",
            Email = request.Email,
            UserId = userId
        };
    }

    public static RegisterShelterResponse ToRegisterResponse(ShelterEntity shelter)
    {
        return new RegisterShelterResponse
        {
            Id = shelter.Id,
            Name = shelter.Name,
            Description = shelter.Description,
            Email = shelter.Email,
            UserId = shelter.UserId
        };
    }
}