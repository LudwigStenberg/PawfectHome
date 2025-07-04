public class AdoptionApplicationEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public AdoptionStatus AdoptionStatus { get; set; } = AdoptionStatus.Pending;
    public required string UserId { get; set; }
    public int PetId { get; set; }

    // Navigation Props
    public UserEntity User { get; set; } = null!; // Many-to-One
    public PetEntity Pet { get; set; } = null!; //  Many-to-One
}

public enum AdoptionStatus
{
    Pending,
    Declined,
    Approved,
}
