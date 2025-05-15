public class RegisterPetResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime Birthdate { get; set; }
    public required Gender Gender { get; set; }
    public required Species Species { get; set; }
    public required string Breed { get; set; }
    public required string Description { get; set; }
    public string? ImageURL { get; set; }
    public required bool IsNeutered { get; set; }
    public required bool HasPedigree { get; set; }
    public required int ShelterId { get; set; }
    public DateTime CreatedAt { get; set; }
}
