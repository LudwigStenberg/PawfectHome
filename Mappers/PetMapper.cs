
public static class PetMapper
{
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