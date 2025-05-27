public static class AdoptionApplicationMapper
{
    public static AdoptionApplicationEntity ToEntity(RegisterAdoptionRequest request, string userId)
    {
        return new AdoptionApplicationEntity { PetId = request.PetId, UserId = userId };
    }

    public static RegisterAdoptionResponse ToRegisterResponse(
        AdoptionApplicationEntity adoptionApplication
    )
    {
        return new RegisterAdoptionResponse
        {
            Id = adoptionApplication.Id,
            CreatedDate = adoptionApplication.CreatedDate,
            AdoptionStatus = adoptionApplication.AdoptionStatus,
            UserId = adoptionApplication.UserId,
            PetId = adoptionApplication.PetId,
        };
    }

    public static GetAdoptionApplicationResponse ToGetResponse(
        AdoptionApplicationEntity adoptionApplication
    )
    {
        return new GetAdoptionApplicationResponse
        {
            Id = adoptionApplication.Id,
            CreatedDate = adoptionApplication.CreatedDate,
            AdoptionStatus = adoptionApplication.AdoptionStatus,
            UserId = adoptionApplication.UserId,
            PetId = adoptionApplication.PetId,
            PetName = adoptionApplication.Pet?.Name ?? "Unknown Pet",
        };
    }
}
