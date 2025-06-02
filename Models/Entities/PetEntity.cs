public class PetEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime Birthdate { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public Species Species { get; set; } = Species.Undefined;
    public string Breed { get; set; } = "Undefined";
    public string Description { get; set; } = "No description";
    public string? ImageURL { get; set; }
    public bool IsNeutered { get; set; } = false;
    public bool HasPedigree { get; set; } = false;
    public required int ShelterId { get; set; }

    // Navigation Props
    public ShelterEntity Shelter { get; set; } = null!; // Many-to-One
    public ICollection<AdoptionApplicationEntity> AdoptionApplications { get; set; } =
        new List<AdoptionApplicationEntity>(); // One-to-Many
}

public enum Gender
{
    Unknown, // Default
    Male,
    Female,
}

public enum Species
{
    Undefined, // Default
    Cat,
    Dog,
}
