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

    public static ShelterDetailResponse ToDetailResponse(ShelterEntity shelter)
    {
        return new ShelterDetailResponse
        {
            Id = shelter.Id,
            Name = shelter.Name,
            Description = shelter.Description,
            Email = shelter.Email,
            UserId = shelter.UserId,

            Pets = shelter.Pets.Select(PetMapper.ToSummaryResponse).ToList()
        };
    }

    public static ShelterSummaryResponse ToSummaryResponse(ShelterWithPetCountDto shelter)
    {
        return new ShelterSummaryResponse
        {
            Id = shelter.Id,
            Name = shelter.Name,
            Description = shelter.Description,
            Email = shelter.Email,
            PetCount = shelter.PetCount
        };
    }

    public static ShelterSummary ToSummary(ShelterEntity shelter)
    {
        return new ShelterSummary
        {
            Id = shelter.Id,
            Name = shelter.Name ?? "Unknown",
            Description = shelter.Description ?? "No description",
            Email = shelter.Email ?? "noemail@example.com"
        };
    }
}