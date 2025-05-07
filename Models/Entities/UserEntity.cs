
using Microsoft.AspNetCore.Identity;

public class UserEntity : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int ShelterId { get; set; }

    // Navigation prop
    public ShelterEntity Shelter { get; set; } // One-to-One
    public ICollection<AdoptionApplicationEntity> AdoptionApplications { get; set; } = new List<AdoptionApplicationEntity>(); // One-to-Many
}
