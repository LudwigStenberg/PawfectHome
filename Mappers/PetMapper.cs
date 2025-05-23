
public static class PetMapper
{
    public static PetEntity ToEntity(RegisterPetRequest request, DateTime utcBirthdate)
    {
        return new PetEntity
        {
            Name = request.Name,
            Birthdate = utcBirthdate,
            Gender = request.Gender,
            Species = request.Species,
            Breed = request.Breed,
            Description = request.Description,
            ImageURL = request.ImageURL,
            IsNeutered = request.IsNeutured,
            HasPedigree = request.HasPedigree,
            ShelterId = request.ShelterId,
        };
    }

    public static PetSummaryResponse ToPetSummaryResponse(PetEntity pet)
    {
        return new PetSummaryResponse
        {
            Id = pet.Id,
            Name = pet.Name,
            Birthdate = pet.Birthdate,
            Gender = pet.Gender,
            Species = pet.Species,
            ImageURL = pet.ImageURL
        };

    }
}