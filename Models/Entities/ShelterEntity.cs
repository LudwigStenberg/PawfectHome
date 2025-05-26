public class ShelterEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = "No description";
    public required string Email { get; set; }
    public required string UserId { get; set; }

    // Navigation props
    public UserEntity? User { get; set; } // One-to-One
    public ICollection<PetEntity> Pets { get; set; } = new List<PetEntity>(); // One-to-Many
}
