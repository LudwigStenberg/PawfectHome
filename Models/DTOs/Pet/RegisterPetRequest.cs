public class RegisterPetRequest
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public Species Species { get; set; } = Species.Undefined;
    public string Breed { get; set; } = "Undefined";
    public string Description { get; set; } = "No description";
    public string? ImageURL { get; set; }
    public bool IsNeutured { get; set; } = false;
    public bool HasPedigree { get; set; } = false;
    public int ShelterId { get; set; }
}
