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

    public static AdoptionApplicationShelterSummary ToUpdateResponse(
        AdoptionApplicationEntity updatedApplication
    )
    {
        return new AdoptionApplicationShelterSummary
        {
            Id = updatedApplication.Id,
            CreatedDate = updatedApplication.CreatedDate,
            AdoptionStatus = updatedApplication.AdoptionStatus,
            ApplicantName =
                $"{updatedApplication.User?.FirstName} {updatedApplication.User?.LastName}".Trim(),
            PetName = updatedApplication.Pet?.Name ?? "",
            PetId = updatedApplication.Pet?.Id ?? 0,
        };
    }
}
