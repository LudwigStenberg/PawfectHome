
public static class PetMapper
{
    public static PetEntity ToEntity(RegisterPetRequest pet, DateTime utcBirthdate)
    {
        return new PetEntity
        {
            Name = pet.Name,
            Birthdate = utcBirthdate,
            Gender = pet.Gender,
            Species = pet.Species,
            Breed = pet.Breed,
            Description = pet.Description,
            ImageURL = pet.ImageURL,
            IsNeutered = pet.IsNeutured,
            HasPedigree = pet.HasPedigree,
            ShelterId = pet.ShelterId,
        };
    }

    public static RegisterPetResponse ToRegisterResponse(PetEntity pet)
    {

    }

    public static PetSummaryResponse ToSummaryResponse(PetEntity pet)
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