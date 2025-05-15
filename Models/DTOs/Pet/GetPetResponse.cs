public class GetPetResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Birthdate { get; set; }
    public Gender Gender { get; set; }
    public Species Species { get; set; }
    public string Breed { get; set; }
    public string Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsNeutured { get; set; }
    public bool HasPedigree { get; set; }
    public int ShelterId { get; set; }

    public ShelterSummary? Shelter { get; set; }
}

public class ShelterSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
