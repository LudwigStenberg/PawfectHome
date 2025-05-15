public class ShelterWithPetCountDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Email { get; set; }
    public int PetCount { get; set; }
}