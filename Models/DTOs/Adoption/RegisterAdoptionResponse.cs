public class RegisterAdoptionResponse
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public AdoptionStatus AdoptionStatus { get; set; } = AdoptionStatus.Pending;
    public required string UserId { get; set; }
    public int PetId { get; set; }
}
