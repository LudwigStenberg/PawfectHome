public class GetPetResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime Birthdate { get; set; }
    public Gender Gender { get; set; }
    public Species Species { get; set; }
    public required string Breed { get; set; }
    public required string Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsNeutured { get; set; }
    public bool HasPedigree { get; set; }
    public int ShelterId { get; set; }

    public ShelterSummary? Shelter { get; set; }
}
