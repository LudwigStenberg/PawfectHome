using Microsoft.AspNetCore.Identity;

public class UserEntity : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Navigation prop
    public ShelterEntity Shelter { get; set; } = null!; // One-to-One
    public ICollection<AdoptionApplicationEntity> AdoptionApplications { get; set; } =
        new List<AdoptionApplicationEntity>(); // One-to-Many
}
