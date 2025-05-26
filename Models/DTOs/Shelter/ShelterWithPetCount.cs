/// <summary>
/// Internal DTO used for data transfer between repository and service layers.
/// Contains shelter data enriched with pet count information.
/// </summary>
public class ShelterWithPetCount
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Email { get; set; }
    public int PetCount { get; set; }
}
