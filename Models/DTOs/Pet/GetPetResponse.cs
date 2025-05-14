public class GetPetResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public Species Species { get; set; }
    public string Breed { get; set; }
    public string Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsNeutured { get; set; }
    public bool HasPedigree { get; set; }
    public int ShelterId { get; set; }

    public ShelterSummaryResponse? shelter { get; set; }
}

public class ShelterSummaryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; }
    public required string Email { get; set; }
}
