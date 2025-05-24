
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
        return new RegisterPetResponse
        {
            Id = pet.Id,
            Name = pet.Name,
            Birthdate = pet.Birthdate,
            Gender = pet.Gender,
            Species = pet.Species,
            Breed = pet.Breed,
            Description = pet.Description,
            ImageURL = pet.ImageURL,
            IsNeutered = pet.IsNeutered,
            HasPedigree = pet.HasPedigree,
            ShelterId = pet.ShelterId,
            CreatedAt = DateTime.UtcNow // This needs to be changed if the PetEntity gets a 'CreatedAt' property.
        };
    }

    public static GetPetResponse ToGetResponse(PetEntity pet)
    {
        return new GetPetResponse
        {
            Id = pet.Id,
            Name = pet.Name,
            Birthdate = pet.Birthdate,
            Gender = pet.Gender,
            Species = pet.Species,
            Breed = pet.Breed,
            Description = pet.Description,
            ImageURL = pet.ImageURL,
            IsNeutured = pet.IsNeutered,
            HasPedigree = pet.HasPedigree,
            ShelterId = pet.ShelterId,
            Shelter = pet.Shelter != null ? ShelterMapper.ToSummary(pet.Shelter) : null
        };
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