
public class PetSummaryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime Birthdate { get; set; }
    public required Gender Gender { get; set; }
    public required Species Species { get; set; }
    public string? ImageURL { get; set; }
}